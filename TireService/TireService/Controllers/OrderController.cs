using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using TireService.Models;
using TireService.Services;
using TireService.xlsOrders;

namespace TireService.Controllers;
// Контроллер Заказ

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly ServiceService _serviceService;
    private readonly WorkerService _workerService;
    private readonly UserService _userService;
    private readonly IOptions<EmailConfig> _email;
    
    public OrderController(OrderService orderService, ServiceService serviceService, 
        WorkerService workerService, UserService userService, IOptions<EmailConfig> email)
    {
        _orderService = orderService;
        _serviceService = serviceService;
        _workerService = workerService;
        _userService = userService;
        _email = email;
    }

    [HttpGet, Authorize(Roles = "admin")]
    public async Task<List<Order>> GetAll() =>
        await _orderService.GetAllAsync();

    // Фильтр по филлиалу по id
    [HttpGet, Authorize(Roles = "admin, manager")]
    [Route("getAllByBranch")]
    public async Task<List<Order>> GetAllByBranch()
    {
        var username = User.Identity!.Name;
        if (username == null) return null!;
        
        var user = await _userService.GetAsync(username);
        
        if (user?.BranchId?.Id == null) return null!;
        
        var temp = await _orderService.GetByBranch(user.BranchId.Id);
        foreach (var equipment in from order in temp from service in order.ServiceId! from equipment in service!.EquipmentId! select equipment)
        {
            equipment!.EstimatedValue = null;
        }
        return temp;
    }
    
    // Фильтр текущих заказов по филлиалу по id
    [HttpGet, Authorize(Roles = "admin, manager")]
    [Route("getInProcessOrPlanByBranch")]
    public async Task<List<Order>> GetStatusByBranch()
    {
        var username = User.Identity!.Name;
        if (username == null) return null!;
        
        var user = await _userService.GetAsync(username);
        
        if (user?.BranchId?.Id == null) return null!;
        
        return await _orderService.GetByBranchInProcessOrPlan(user.BranchId.Id);
    }

    // Фильтр прошлых заказов по филлиалу по id
    [HttpGet, Authorize(Roles = "admin, manager")]
    [Route("getPastByBranch")]
    public async Task<List<Order>> GetPast()
    {
        var username = User.Identity!.Name;
        if (username == null) return null!;
        
        var user = await _userService.GetAsync(username);
        
        if (user?.BranchId?.Id == null) return null!;
        
        return await _orderService.GetPastByBranch(user.BranchId.Id);
    }

    // Фильтр прошлых заказов по филлиалу по id
    [HttpGet, Authorize(Roles = "admin, manager")]
    [Route("getByStatus")]
    public async Task<List<Order>> GetStatus(string status)
    {
        var username = User.Identity!.Name;
        if (username == null) return null!;
        
        var user = await _userService.GetAsync(username);
        
        if (user?.BranchId?.Id == null) return null!;

        return await _orderService.GetByBranchStatus(user.BranchId.Id, status);
    }
    
    [HttpPost, Authorize(Roles = "admin, manager")]
    [Route("changeStatus")]
    public async Task<IActionResult> ChangeStatus(string id, string status)
    {
        var order = await _orderService.GetAsync(id);
        
        if (order is null || order.Deleted == true || status == order.Status)
            return NotFound("Не возможно поменять статус заказа");

        if (status is not ("Plan" or "InProcess" or "Cancel" or "Success" or "Unsuccess"))
            return NotFound("Не возможно поменять статус заказа");
        
        order.Status = status;
        await _orderService.UpdateAsync(id, order);
        return NoContent();
    }
    
    [HttpPost, Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> Post(Order newOrder)
    {
        if (newOrder.StartDate == null 
            || newOrder.WorkerId == null 
            || newOrder.ServiceId == null)
            return NotFound("Не заполнены поля. Обязательные поля:\n\t StartDate, WorkerId, ServiceId, Cost>0");
        
        
        var username = User.Identity!.Name;
        if (username == null) return NotFound("Please Log In");
        var user = await _userService.GetAsync(username);
        if (user?.BranchId?.Id == null) return NotFound("User does not have BranchId");
        
        // Вычисляем и записываем дату окончания заказа
        var allDurationInMinutes = 0;
        var servicesByBranch = await _serviceService.GetByBranch(user.BranchId.Id);

        if (servicesByBranch != null)
            foreach (var service in servicesByBranch.Where(_ => newOrder.ServiceId != null))
                if (newOrder.ServiceId != null)
                    for (var i = 0; i < newOrder.ServiceId.Length; i++)
                        if (service.Id == newOrder.ServiceId[i]!.Id)
                        {
                            newOrder.ServiceId[i] = service;
                            var durationInMinutes = newOrder.ServiceId[i]!.DurationInMinutes;
                            if (durationInMinutes != null)
                                allDurationInMinutes += durationInMinutes.Value;
                        }

        var allOrders = await _orderService.GetByBranch(user.BranchId.Id);
        DateTime dateNewOrder = (DateTime)newOrder.StartDate;
        var finallyDateNewOrder = dateNewOrder.AddMinutes(allDurationInMinutes);
        
        newOrder.EndDate = finallyDateNewOrder;
            
        // Проверка на занятость сотрудника
        var ordersInProcessByWorker = allOrders.Find(o =>
            o.WorkerId == newOrder.WorkerId
            && o.Status is "InProcess");
        if (ordersInProcessByWorker is not null && ordersInProcessByWorker.EndDate > newOrder.StartDate)
            return NotFound("В данный момент сотрудник занят");

        // проверка можно ли создать заказ во время другого запланированного заказа 
        /*      https://stackoverflow.com/questions/64843394/c-sharp-linq-find-conflicting-times
             *      Давайте абстрагируемся от этого только до двух интервалов AB и CD,
             * где A и C представляют начало, а B и D представляют концы.
             *      Соотвественно C = o.StartDate , D = o.EndDate;
             * A = newOrder.StartDate, B = newOrder.EndDate;
             *      Когда интервалы перекрываются:
             * 1) A <= C и B >= D
             * 2) A >= C и A < D
             * 3) B > C и B <= D
             */
        var ordersInPlanned = allOrders.FindAll(o =>
            o.WorkerId!.Id == newOrder.WorkerId.Id && o.Status is "Plan" &&
            ((newOrder.StartDate <= o.StartDate && newOrder.EndDate >= o.EndDate) ||
             (newOrder.StartDate >= o.StartDate && newOrder.StartDate < o.EndDate) ||
             (newOrder.EndDate > o.StartDate && newOrder.EndDate <= o.EndDate))).ToList();
        bool isAnyInPlanned = ordersInPlanned.Any();
        if (isAnyInPlanned)
            return NotFound("В данный момент у сотрудника запланированный заказ");
            
        // Чтоб можно было вписать только id услуги
        var services = await _serviceService.GetByBranch(user.BranchId.Id);
        var costAll = 0.0;
        if (services != null)
            foreach (var service in services)
            {
                for (int i = 0; i < newOrder.ServiceId!.Length; i++)
                {
                    if (newOrder.ServiceId![i]!.Deleted == true
                        || newOrder.ServiceId![i]!.Id != service.Id) continue;
                    newOrder.ServiceId![i] = service;
                    if (service.Cost != null) costAll += service.Cost.Value;
                }
            }
        
        // Чтоб можно было вписать только id сотрудника
        if (newOrder.WorkerId.Id != null)
        {
            var worker = await _workerService.GetAsync(newOrder.WorkerId.Id);
            newOrder.WorkerId = worker;
        }
        else
            return NotFound("В поле WorkerId неверно введен Id");

        newOrder.Status = "Plan";
        newOrder.Cost = costAll;
        await _orderService.CreateAsync(newOrder);
        return CreatedAtAction(nameof(GetAll), new { id = newOrder.Id }, newOrder);
    }
    
    // Отправка Отчет по оказанию услуг в всех филиалах
    [HttpGet, Authorize(Roles = "admin")]
    [Route("getReportAllOrders")]
    public async Task<IActionResult> ReportAllOrders(string email, DateTime startDate, DateTime endDate)
    {
        var username = User.Identity!.Name;
        var filename = $"Reports/Отчет по оказанию услуг во всех филиалах с " +
                       $"{startDate.Year}_{startDate.Day}_{startDate.Month}_{startDate.Hour}_{startDate.Minute} " +
                       $"по {endDate.Year}_{endDate.Day}_{endDate.Month}_{endDate.Hour}_{endDate.Minute}.xlsx";
        if (username == null) return BadRequest("User is not authorizations");
        var isEnd = await BuildWorkbookAllOrders(startDate, endDate, filename);
        if (isEnd != true) return BadRequest("Отчет еще не сформировался");
        var isSendOnEmail = await SendOnEmail("Отчет по оказанию услуг во всех филиалах",
            filename, email, "null");
        if (isSendOnEmail == false)
        {
            return BadRequest("Message didn't send");
        }
        System.IO.File.Delete(filename);
        
        return Ok();
    }
    
    // Отправка Отчет по оказанию услуг в данном филиале 
    [HttpGet, Authorize(Roles = "admin, manager")]
    [Route("getReportOrder")]
    public async Task<IActionResult> ReportOrder(string email,DateTime startDate, DateTime endDate)
    {
        var username = User.Identity!.Name;
        var filename = $"Reports/Отчет по оказанию услуг в данном филиале с " +
                       $"{startDate.Year}_{startDate.Day}_{startDate.Month}_{startDate.Hour}_{startDate.Minute} " +
                       $"по {endDate.Year}_{endDate.Day}_{endDate.Month}_{endDate.Hour}_{endDate.Minute}.xlsx";
        if (username == null) return BadRequest("User is not authorizations");
        var isEnd = await BuildWorkbookOrder(username, startDate, endDate, filename);
        if (isEnd != true) return BadRequest("Отчет еще не сформировался");
        var isSendOnEmail = await SendOnEmail("Отчет по оказанию услуг в данном филиале",
            filename, email, "null");
        if (isSendOnEmail == false)
        {
            return BadRequest("Message didn't send");
        }
        System.IO.File.Delete(filename);
        
        return Ok();
    }
    
    // Отправка по почте Отчета по работе персонала в данном филиале
    [HttpGet, Authorize(Roles = "admin, manager")]
    [Route("getReportPersonal")]
    public async Task<IActionResult> ReportPersonal(string email, DateTime startDate, DateTime endDate)
    {
        var username = User.Identity!.Name;
        var filename = $"Reports/Отчет по работе персонала c " +
                       $"{startDate.Year}_{startDate.Day}_{startDate.Month}_{startDate.Hour}_{startDate.Minute} " +
                       $"по {endDate.Year}_{endDate.Day}_{endDate.Month}_{endDate.Hour}_{endDate.Minute}.xlsx";
        if (username == null) return BadRequest("User is not authorizations");
        var isEnd = await BuildWorkbookPersonal(username, startDate, endDate, filename);
        if (isEnd != true) return BadRequest("User is not authorizations");
        var isSendOnEmail = await SendOnEmail("Отчет по работе персонала в филиале",
            filename, email, "null");
        if (isSendOnEmail == false)
        {
            return BadRequest("Message didn't send");
        }
        System.IO.File.Delete(filename);
        return Ok();

    }
    
    // Отправка по почте Отчета по работе персонала во всех филиалах
    [HttpGet, Authorize(Roles = "admin")]
    [Route("getReportAllPersonal")]
    public async Task<IActionResult> ReportAllPersonal(string email, DateTime startDate, DateTime endDate)
    {
        var username = User.Identity!.Name;
        var filename = $"Reports/Отчет по работе персонала c " +
                       $"{startDate.Year}_{startDate.Day}_{startDate.Month}_{startDate.Hour}_{startDate.Minute} " +
                       $"по {endDate.Year}_{endDate.Day}_{endDate.Month}_{endDate.Hour}_{endDate.Minute}.xlsx";
        if (username == null) return BadRequest("User is not authorizations");
        var isEnd = await BuildWorkbookAllPersonal(startDate, endDate, filename);
        if (isEnd != true) return BadRequest("User is not authorizations");
        var isSendOnEmail = await SendOnEmail("Отчет по работе персонала во всех филиалах",
            filename, email, "null");
        if (isSendOnEmail == false)
        {
            return BadRequest("Message didn't send");
        }
        System.IO.File.Delete(filename);
        return Ok();

    }
    
    // Отравка по почте
    private async Task<bool> SendOnEmail(string textBody, string path, string toAddress, string username)
    {
        var isGood = true;
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_email.Value.Username, _email.Value.Address));
            message.To.Add(new MailboxAddress(username, toAddress));
            message.Subject = "Отчет";
            var builder = new BodyBuilder();
            builder.TextBody = textBody;
            builder.Attachments.Add(path);
            message.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false);

            //SMTP server authentication if needed
            await client.AuthenticateAsync(_email.Value.Address, _email.Value.Password);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            isGood = false;
            return isGood;
        }
        return isGood;
    }
    
    // Отчет по работе персонала во всех филиалах
    private async Task<bool> BuildWorkbookAllPersonal(DateTime startDate, DateTime endDate, string filename)
    {
        /* По аналогии с методом BuildWorkbookAllOrders и BuildWorkbookPersonal */
        var isEnd = true;
        using var workbook = new XLWorkbook();
        
        var ordersForBranch = await _orderService.GetAllByDate(startDate, endDate);
        var branchesIds = ordersForBranch.Select(order => order.WorkerId?.BranchId).ToList();
        var branches = branchesIds.DistinctBy(b => b?.Id).ToList();
        var countBranch = 2;
        
        var worksheet = workbook.Worksheets.Add("Personal");

        worksheet.Cell(1, 1).Value = "ФИО";
        worksheet.Cell(1, 2).Value = "Филиал";
        worksheet.Cell(1, 3).Value = "Количество выполненных заказов за период";
        worksheet.Cell(1, 4).Value = "количество успешно выполненных заказов за период";
        worksheet.Cell(1, 5).Value = "Количество неуспешно выполненных заказов";
        worksheet.Cell(1, 6).Value = "Стоимость неуспешно выполненных заказов";
        worksheet.Cell(1, 7).Value = "Общая стоимость успешных заказов за период";
        worksheet.Cell(1, 8).Value = "Постоянная заработная плата";
        worksheet.Cell(1, 9).Value = "Бонусные сотрудников";
        worksheet.Cell(1, 10).Value = "Прибыль";
        foreach (var branch in branches)
        {
            var ordersWithoutDistinct = await _orderService.GetAllByDateBranch(branch!.Id!, startDate, endDate);
            var orders = ordersWithoutDistinct.Distinct().ToList();

            var reportOrders = new List<ReportOrder>();
            var sumEstimatedCost = 0.0;
            foreach (var order in orders)
            {
                if (reportOrders.Count == 0)
                {
                    var report = new ReportOrder()
                    {
                        Address = branch.Adress!
                    };
                    report.WorkerId = order.WorkerId!;
                    switch (order.Status)
                    {
                        // Общее кол-во и стомость выполненых заказов
                        case "Success":
                            report.CountSuccesses++;
                            report.CostSuccesses += order.Cost!.Value;
                            break;
                        case "Unsuccess":
                            report.CountUnsuccesses++;
                            report.CostUnsuccesses += order.Cost!.Value;
                            break;
                    }

                    reportOrders.Add(report);
                }
                else
                {
                    foreach (var reportOrder in reportOrders.ToList())
                    {
                        if (reportOrder.WorkerId.Id == order.WorkerId!.Id)
                        {
                            switch (order.Status)
                            {
                                // Общее количество выполненых заказов
                                case "Success":
                                    reportOrder.CountSuccesses++;
                                    reportOrder.CostSuccesses += order.Cost!.Value;
                                    break;
                                case "Unsuccess":
                                    reportOrder.CountUnsuccesses++;
                                    reportOrder.CostUnsuccesses += order.Cost!.Value;
                                    break;
                            }
                        }
                        else
                        {
                            var report = new ReportOrder()
                            {
                                Address = branch.Adress!
                            };
                            report.WorkerId = order.WorkerId!;
                            switch (order.Status)
                            {
                                // Общее количество выполненых заказов
                                case "Success":
                                    report.CountSuccesses++;
                                    report.CostSuccesses += order.Cost!.Value;
                                    break;
                                case "Unsuccess":
                                    report.CountUnsuccesses++;
                                    report.CostUnsuccesses += order.Cost!.Value;
                                    break;
                            }

                            reportOrders.Add(report);
                        }
                    }
                }
            }

            var workers = await _workerService.GetByBranch(branch.Id!);
            

            foreach (var report in reportOrders)
            {
                report.CountAll = report.CountSuccesses + report.CountUnsuccesses;
                report.Amortization = sumEstimatedCost;
                // Постоянная заработная плата 
                if (workers != null)
                    foreach (var worker in workers)
                    {
                        for (int i = 0; i < endDate.Day - startDate.Day; i++)
                        {
                            report.Payroll += worker.PayPerHour!.Value * 8;
                        }
                    }

                // Бонусные сотрудников
                report.BonusPayroll = report.CostSuccesses * 0.1;
                // Прибыль 
                report.Profit = report.Payroll + report.BonusPayroll;

                // Округляем до рублей
                report.CostSuccesses = Math.Round(report.CostSuccesses, 2);
                report.CostUnsuccesses = Math.Round(report.CostUnsuccesses, 2);
                report.Amortization = Math.Round(report.Amortization, 2);
                report.Payroll = Math.Round(report.Payroll, 2);
                report.BonusPayroll = Math.Round(report.BonusPayroll, 2);
                report.Profit = Math.Round(report.Profit, 2);
                
                worksheet.Cell(countBranch, 1).Value =
                    report.WorkerId.FirstName + " " + report.WorkerId.LastName + " " + report.WorkerId.MidName;
                worksheet.Cell(countBranch, 2).Value = report.Address;
                worksheet.Cell(countBranch, 3).Value = report.CountAll;
                worksheet.Cell(countBranch, 4).Value = report.CountSuccesses;
                worksheet.Cell(countBranch, 5).Value = report.CountUnsuccesses;
                worksheet.Cell(countBranch, 6).Value = report.CostUnsuccesses;
                worksheet.Cell(countBranch, 7).Value = report.CostSuccesses;
                worksheet.Cell(countBranch, 8).Value = report.Payroll;
                worksheet.Cell(countBranch, 9).Value = report.BonusPayroll;
                worksheet.Cell(countBranch, 10).Value = report.Profit;
                countBranch++;
            }
        }

        workbook.SaveAs(filename);
        return isEnd;
    }
    
    // Отчет по работе персонала в данном филиале
    private async Task<bool> BuildWorkbookPersonal(string username, DateTime startDate, DateTime endDate, string filename)
    {
        /* Логика такая:
            1. Для начала создаем Excel Книгу и в ней Лист. Так же ищем пользователя по id, 
        чтоб найти заказы по его филиалу.
            2. Сразу разметим Столбцы по условиям.
            3. Создаем массив объектов ReportOrder в который мы будем загружать данные.
            
            4. Если это первый наш ReportOrder, тогда 
        создаем данный объект и сразу записываем Адресс, Сотрудника и 
        общее кол-во и стомость выполненых заказов. Добавляем в этот массив reportOrders
            5. Иначе пробегаемся циклом по нашему массиву reportOrders. 
        Если сотрудник из нашего массива совпадает с сотрудником из заказа, тогда 
        считается общее кол-во и стомость выполненых заказов, 
        иначе создается новый ReportOrder и считается тоже самое.
            6. По аналогии с Отчетом по оказанию услуг в данном филиале.
            
            Дополнительно: Создана затычка для отслежки что Отчет создался, 
        в противном случае на почту ничего не придет.
         */
        var isEnd = true;
        using var workbook = new XLWorkbook();
        var user = await _userService.GetAsync(username);
        var orders = await _orderService.GetAllByDateBranch(user?.BranchId?.Id!, startDate, endDate);
    
        var worksheet = workbook.Worksheets.Add("Personal");

        worksheet.Cell(1, 1).Value = "ФИО";
        worksheet.Cell(1, 2).Value = "Филиал";
        worksheet.Cell(1, 3).Value = "Количество выполненных заказов за период";
        worksheet.Cell(1, 4).Value = "количество успешно выполненных заказов за период";
        worksheet.Cell(1, 5).Value = "Количество неуспешно выполненных заказов";
        worksheet.Cell(1, 6).Value = "Стоимость неуспешно выполненных заказов";
        worksheet.Cell(1, 7).Value = "Общая стоимость успешных заказов за период";
        worksheet.Cell(1, 8).Value = "Постоянная заработная плата";
        worksheet.Cell(1, 9).Value = "Бонусные сотрудников";
        worksheet.Cell(1, 10).Value = "Прибыль";
    
        var reportOrders = new List<ReportOrder>();
        var sumEstimatedCost = 0.0;
        foreach (var order in orders)
        {
            if (reportOrders.Count == 0)
            {
                var report = new ReportOrder()
                {
                    Address = user?.BranchId?.Adress!
                };
                report.WorkerId = order.WorkerId!;
                switch (order.Status)
                {
                    // Общее кол-во и стомость выполненых заказов
                    case "Success":
                        report.CountSuccesses++;
                        report.CostSuccesses += order.Cost!.Value;
                        break;
                    case "Unsuccess":
                        report.CountUnsuccesses++;
                        report.CostUnsuccesses += order.Cost!.Value;
                        break;
                }
                reportOrders.Add(report);
            }
            else
            {
                foreach (var reportOrder in reportOrders.ToList())
                {
                    if (reportOrder.WorkerId.Id == order.WorkerId!.Id)
                    {
                        switch (order.Status)
                        {
                            // Общее количество выполненых заказов
                            case "Success":
                                reportOrder.CountSuccesses++;
                                reportOrder.CostSuccesses += order.Cost!.Value;
                                break;
                            case "Unsuccess":
                                reportOrder.CountUnsuccesses++;
                                reportOrder.CostUnsuccesses += order.Cost!.Value;
                                break;
                        }
                    }
                    else
                    {
                        var report = new ReportOrder()
                        {
                            Address = user?.BranchId?.Adress!
                        };
                        report.WorkerId = order.WorkerId!;
                        switch (order.Status)
                        {
                            // Общее количество выполненых заказов
                            case "Success":
                                report.CountSuccesses++;
                                report.CostSuccesses += order.Cost!.Value;
                                break;
                            case "Unsuccess":
                                report.CountUnsuccesses++;
                                report.CostUnsuccesses += order.Cost!.Value;
                                break;
                        }

                        reportOrders.Add(report);
                    }
                }
            }
        }
    
        var workers = await _workerService.GetByBranch(user?.BranchId?.Id!);
        var counterRows = 1;
    
        foreach (var report in reportOrders)
        {
            report.CountAll = report.CountSuccesses + report.CountUnsuccesses;
            report.Amortization = sumEstimatedCost;
            // Постоянная заработная плата 
            if (workers != null)
                foreach (var worker in workers)
                {
                    for (int i = 0; i < endDate.Day - startDate.Day; i++)
                    {
                        report.Payroll += worker.PayPerHour!.Value * 8;
                    }
                }
            // Бонусные сотрудников
            report.BonusPayroll = report.CostSuccesses * 0.1;
            // Прибыль 
            report.Profit = report.Payroll + report.BonusPayroll;
    
            // Округляем до рублей
            report.CostSuccesses = Math.Round(report.CostSuccesses, 2);
            report.CostUnsuccesses = Math.Round(report.CostUnsuccesses, 2);
            report.Amortization = Math.Round(report.Amortization, 2);
            report.Payroll = Math.Round(report.Payroll, 2);
            report.BonusPayroll = Math.Round(report.BonusPayroll, 2);
            report.Profit = Math.Round(report.Profit, 2);
            counterRows++;
            worksheet.Cell(counterRows, 1).Value = 
                report.WorkerId.FirstName + " " + report.WorkerId.LastName + " " + report.WorkerId.MidName;
            worksheet.Cell(counterRows, 2).Value = report.Address;
            worksheet.Cell(counterRows, 3).Value = report.CountAll;
            worksheet.Cell(counterRows, 4).Value = report.CountSuccesses;
            worksheet.Cell(counterRows, 5).Value = report.CountUnsuccesses;
            worksheet.Cell(counterRows, 6).Value = report.CostUnsuccesses;
            worksheet.Cell(counterRows, 7).Value = report.CostSuccesses;
            worksheet.Cell(counterRows, 8).Value = report.Payroll;
            worksheet.Cell(counterRows, 9).Value = report.BonusPayroll;
            worksheet.Cell(counterRows, 10).Value = report.Profit;
        }
        workbook.SaveAs(filename);
        return isEnd;
    }
    
    // Отчет по оказанию услуг в данном филиале 
    private async Task<bool> BuildWorkbookOrder(string username, DateTime startDate, DateTime endDate, string filename)
    {
        /* Логика такая:
            1. Для начала создаем Excel Книгу и в ней Лист. Так же ищем пользователя по id, 
        чтоб найти заказы по его филиалу.
            2. Берем первый за пример чтобы записать сразу Адресс.
            3. Создаем объект ReportOrder в который мы будем загружать данные.
            4. Циклом пробегаемся по найденым заказам. Считаем кол-во и стоимость 
        Успешных и Неуспешных заказов.
            5. Циклом пробегаемся по Услугам и в услугах циклом пробегаемся по Оборудованиям 
        в найденных заказах, и считаем Амортизацию как в ТЗ.
            6. Записываем в наш объект report все найденные параметры.
            7. Вычисление постоянной зп. Берем из юзера который обращается Id филиала и 
        делаем запрос к бд в коллекцию Сотрудников (Worker). И если сотрудники есть, то 
        циклом пробегаемся по найденным сотрудникам и внутри делаем еще цикл с условием кол-ва дней периода
        и умножаем зп в час на 8 (вообщем как в ТЗ).
            8. Так как у нас численные значения Double, мы его округляем до 2 после запятой.
            
            Дополнительно: Создана затычка для отслежки что Отчет создался, 
        в противном случае на почту ничего не придет.
         */
        var isEnd = true;
        using var workbook = new XLWorkbook();
        
        var user = await _userService.GetAsync(username);
        var orders = await _orderService.GetAllByDateBranch(user?.BranchId?.Id!, startDate, endDate);
        var orderPrimer = orders.First();
        var worksheet = workbook.Worksheets.Add("Orders");
        
        var report = new ReportOrder
        {
            Address = orderPrimer.WorkerId?.BranchId?.Adress!
        };
    
        var sumEstimatedCost = 0.0;
        foreach (var order in orders)
        {
            // Общее количество выполненых заказов
            switch (order.Status)
            {
                case "Success":
                    report.CountSuccesses++;
                    report.CostSuccesses += order.Cost!.Value;
                    break;
                case "Unsuccess":
                    report.CountUnsuccesses++;
                    report.CostUnsuccesses += order.Cost!.Value;
                    break;
            }
            // Амортизация оборудования на филиале
            foreach (var service in order.ServiceId!)
            {
                foreach (var equipment in service!.EquipmentId!)
                {
                    var startEstMonth = 0.0;
                    var endEstMonth = 0.0;
                    foreach (var estimated in equipment!.EstimatedValue!)
                    {
                        if (startDate.Month != endDate.Month)
                        {
                            if (estimated.Month!.Value.Month == startDate.Month)
                                startEstMonth = estimated.EstimatedCost!.Value;

                            if (estimated.Month.Value.Month == endDate.Month)
                                endEstMonth = estimated.EstimatedCost!.Value;
                        }
                        else
                            sumEstimatedCost += estimated.Amortization!.Value;
                    }
                    sumEstimatedCost += startEstMonth - endEstMonth;
                }
            }
        }
    
        report.CountAll = report.CountSuccesses + report.CountUnsuccesses;
        report.Amortization = sumEstimatedCost;
        // Постоянная заработная плата
        var workers = await _workerService.GetByBranch(user?.BranchId?.Id!);
        if (workers != null)
            foreach (var worker in workers)
            {
                for (var i = 0; i < endDate.Day - startDate.Day; i++)
                {
                    report.Payroll += worker.PayPerHour!.Value * 8;
                }
            }
    
        // Бонусные сотрудников
        report.BonusPayroll = report.CostSuccesses * 0.1;
        // Прибыль 
        report.Profit = report.CostSuccesses - report.Payroll - report.Amortization 
                        - report.BonusPayroll - report.CostUnsuccesses;
    
        // Округляем до рублей
        report.CostSuccesses = Math.Round(report.CostSuccesses, 2);
        report.CostUnsuccesses = Math.Round(report.CostUnsuccesses, 2);
        report.Amortization = Math.Round(report.Amortization, 2);
        report.Payroll = Math.Round(report.Payroll, 2);
        report.BonusPayroll = Math.Round(report.BonusPayroll, 2);
        report.Profit = Math.Round(report.Profit, 2);

        worksheet.Cell(1, 1).Value = "Филиал";
        worksheet.Cell(2, 1).Value = report.Address;
    
        worksheet.Cell(1, 2).Value = "Кол-во выполненных заказов за период";
        worksheet.Cell(2, 2).Value = report.CountAll;

        worksheet.Cell(1, 3).Value = "Кол-во успешно выполненных заказов за период";
        worksheet.Cell(2, 3).Value = report.CountSuccesses;
    
        worksheet.Cell(1, 4).Value = "Кол-во неуспешно выполненных заказов";
        worksheet.Cell(2, 4).Value = report.CountUnsuccesses;
    
        worksheet.Cell(1, 5).Value = "Стоимость неуспешно выполненных заказов";
        worksheet.Cell(2, 5).Value = report.CostUnsuccesses;
    
        worksheet.Cell(1, 6).Value = "Общая стоимость успешных заказов за период";
        worksheet.Cell(2, 6).Value = report.CostSuccesses;
    
        worksheet.Cell(1, 7).Value = "Амортизация оборудования на филиале";
        worksheet.Cell(2, 7).Value = report.Amortization;
    
        worksheet.Cell(1, 8).Value = "Постоянная заработная плата";
        worksheet.Cell(2, 8).Value = report.Payroll;
    
        worksheet.Cell(1, 9).Value = "Бонусные сотрудников";
        worksheet.Cell(2, 9).Value = report.BonusPayroll;
    
        worksheet.Cell(1, 10).Value = "Прибыль";
        worksheet.Cell(2, 10).Value = report.Profit;
        
        workbook.SaveAs(filename);
        return isEnd;
    }
    
    // Отчет по оказанию услуг в всех филиалах 
    private async Task<bool> BuildWorkbookAllOrders(DateTime startDate, DateTime endDate, string filename)
    {
        var isEnd = true;
        using var workbook = new XLWorkbook();
        /* Логика такая:
            1. Для начала нам необходимо посчитать сколько филиалов в данном периоде времени.
        Как это происходит? Мы берем все заказы в периоде, и у них берем Филиал как объект.
        Далее отсикаем дубликаты Филиалов. (Убираем лишние Филлиалы)
            2. Пробегаемся по всем Филиалам, создавая наш объект для заказов ReportOrder вписывая сразу адресс.
         Далее считаем всё необходимое для отчета. (см. Отчет по оказанию услуг в данном филиале ).
            3. Чтоб были строчки вниз, мы создали счетчик филиала countBranch.
            */
        var ordersForBranch = await _orderService.GetAllByDate(startDate, endDate);
        var worksheet = workbook.Worksheets.Add("Orders");
        
        var branchesIds = ordersForBranch.Select(order => order.WorkerId?.BranchId).ToList();
        var branches = branchesIds.DistinctBy(b => b?.Id).ToList();
        var countBranch = 2;
        foreach (var branch in branches)
        {
            var ordersWithoutDistinct = await _orderService.GetAllByDateBranch(branch!.Id!, startDate, endDate);
            var orders = ordersWithoutDistinct.Distinct().ToList();
            var report = new ReportOrder
            {
                Address = branch.Adress!
            };

            var sumEstimatedCost = 0.0;
            foreach (var order in orders)
            {
                // Общее количество выполненых заказов
                switch (order.Status)
                {
                    case "Success":
                        report.CountSuccesses++;
                        report.CostSuccesses += order.Cost!.Value;
                        break;
                    case "Unsuccess":
                        report.CountUnsuccesses++;
                        report.CostUnsuccesses += order.Cost!.Value;
                        break;
                }

                // Амортизация оборудования на филиале
                foreach (var service in order.ServiceId!)
                {
                    foreach (var equipment in service!.EquipmentId!)
                    {
                        var startEstMonth = 0.0;
                        var endEstMonth = 0.0;
                        foreach (var estimated in equipment!.EstimatedValue!)
                        {
                            if (startDate.Month != endDate.Month)
                            {
                                if (estimated.Month!.Value.Month == startDate.Month)
                                    startEstMonth = estimated.EstimatedCost!.Value;

                                if (estimated.Month.Value.Month == endDate.Month)
                                    endEstMonth = estimated.EstimatedCost!.Value;
                            }
                            else
                                sumEstimatedCost += estimated.Amortization!.Value;
                        }

                        sumEstimatedCost += startEstMonth - endEstMonth;
                    }
                }
            }

            report.CountAll = report.CountSuccesses + report.CountUnsuccesses;
            report.Amortization = sumEstimatedCost;
            // Постоянная заработная плата
            if (branch.Id != null)
            {
                var workers = await _workerService.GetByBranch(branch.Id);
                if (workers != null)
                    foreach (var worker in workers)
                    {
                        for (var i = 0; i < endDate.Day - startDate.Day; i++)
                        {
                            report.Payroll += worker.PayPerHour!.Value * 8;
                        }
                    }
            }

            // Бонусные сотрудников
            report.BonusPayroll = report.CostSuccesses * 0.1;
            // Прибыль 
            report.Profit = report.CostSuccesses - report.Payroll - report.Amortization
                            - report.BonusPayroll - report.CostUnsuccesses;

            // Округляем до рублей
            report.CostSuccesses = Math.Round(report.CostSuccesses, 2);
            report.CostUnsuccesses = Math.Round(report.CostUnsuccesses, 2);
            report.Amortization = Math.Round(report.Amortization, 2);
            report.Payroll = Math.Round(report.Payroll, 2);
            report.BonusPayroll = Math.Round(report.BonusPayroll, 2);
            report.Profit = Math.Round(report.Profit, 2);

            worksheet.Cell(1, 1).Value = "Филиал";
            worksheet.Cell(countBranch, 1).Value = report.Address;

            worksheet.Cell(1, 2).Value = "Кол-во выполненных заказов за период";
            worksheet.Cell(countBranch, 2).Value = report.CountAll;

            worksheet.Cell(1, 3).Value = "Кол-во успешно выполненных заказов за период";
            worksheet.Cell(countBranch, 3).Value = report.CountSuccesses;

            worksheet.Cell(1, 4).Value = "Кол-во неуспешно выполненных заказов";
            worksheet.Cell(countBranch, 4).Value = report.CountUnsuccesses;

            worksheet.Cell(1, 5).Value = "Стоимость неуспешно выполненных заказов";
            worksheet.Cell(countBranch, 5).Value = report.CostUnsuccesses;

            worksheet.Cell(1, 6).Value = "Общая стоимость успешных заказов за период";
            worksheet.Cell(countBranch, 6).Value = report.CostSuccesses;

            worksheet.Cell(1, 7).Value = "Амортизация оборудования на филиале";
            worksheet.Cell(countBranch, 7).Value = report.Amortization;

            worksheet.Cell(1, 8).Value = "Постоянная заработная плата";
            worksheet.Cell(countBranch, 8).Value = report.Payroll;

            worksheet.Cell(1, 9).Value = "Бонусные сотрудников";
            worksheet.Cell(countBranch, 9).Value = report.BonusPayroll;

            worksheet.Cell(1, 10).Value = "Прибыль";
            worksheet.Cell(countBranch, 10).Value = report.Profit;

            countBranch++;
        }

        workbook.SaveAs(filename);
        return isEnd;
    }
    
    [HttpPut, Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> Update( Order updatedOrder)
    {
        var id = updatedOrder.Id;
        if (id == null) return NotFound("WRONG: Id не указано");
        
        var order = await _orderService.GetAsync(id);

        if (order is null || order.Deleted == true)
        {
            return NotFound("Невозможно изменить заказ.\n\t Запись удалена или не найдена");
        }
        
        updatedOrder.Id = order.Id;
        
        // Если задан только id сотрудника
        // Затычка если ничего не ввели в эти поля

        if (updatedOrder.WorkerId is not null
            && updatedOrder.WorkerId.Deleted != true
            && updatedOrder.WorkerId.Id != null)
        {
            var idWorkerId = updatedOrder.WorkerId.Id;
            var worker = await _workerService.GetAsync(idWorkerId);
            updatedOrder.WorkerId = worker;
        }
        else
            updatedOrder.WorkerId = order.WorkerId;
        // Проверка чтоб услуг было не больше 5
        // Проходим по каждому заданному id услуги. Ищем в бд услугу по заданному id услуги
        if (updatedOrder.ServiceId is { Length: <= 5 })
        {
            var lenght = updatedOrder.ServiceId.Length;
            var services = new Service?[lenght];
            for (int i = 0; i < lenght; i++)
            {
                var idService = updatedOrder.ServiceId[i]!.Id;
                if (updatedOrder.ServiceId[i]!.Deleted == true
                    || idService == null) continue;
                var serv = await _serviceService.GetAsync(idService);
                services[i] = serv;
            }

            updatedOrder.ServiceId = services;
        }
        else
            return NotFound("WRONG: 1) Услуг должно быть не больше 5 \n\t 2) Введите id услуги");
        
        await _orderService.UpdateAsync(id, updatedOrder);
        
        return NoContent();
    }

    [HttpDelete("{id:length(24)}"), Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var order = await _orderService.GetAsync(id);

        if (order is null || order.Deleted == true)
        {
            return NotFound();
        }
        // Меняем поле на удаленное, а запись перезаписываем
        order.Deleted = true;
        await _orderService.UpdateAsync(id, order);

        return NoContent();
    }
}