using System.Net;
using TireServiceApplication.Source.Models;

namespace TireServiceApplication.Source.Pages.Reports;

public partial class ReportsPage : ContentPage
{
    public ReportsPage()
    {
        InitializeComponent();
    }

    // Отчет по сотрудникам в филиале пользователя
    private async void PersonalReportButton_OnClicked(object? sender, EventArgs e)
    {
        var report = await OrderModel.GetReportPersonal(EmailEntry.Text,DatePickerStart.Date, DatePickerEnd.Date);
        if (report.StatusCode == HttpStatusCode.OK)
        {
            await DisplayAlert("Внимание", "Отчет отправлен.", "Oк");
        }
        else
        {
            await DisplayAlert("Внимание", report.Content, "Oк");
        }
    }

    // Отчет по заказам в филиале пользователя
    private async void OrdersReportButton_OnClicked(object? sender, EventArgs e)
    {
        var report = await OrderModel.GetReportOrder(EmailEntry.Text,DatePickerStart.Date, DatePickerEnd.Date);
        if (report.StatusCode == HttpStatusCode.OK)
        {
            await DisplayAlert("Внимание", "Отчет отправлен.", "Oк");
        }
        else
        {
            await DisplayAlert("Внимание", report.Content, "Oк");
        }
    }

    // Отчет по заказам во всех филиалах
    private async void OrdersReportAllBranchButton_OnClicked(object sender, EventArgs e)
    {
        var report = await OrderModel.GetAllReportOrder(EmailEntry.Text, DatePickerStart.Date, DatePickerEnd.Date);
        if (report.StatusCode == HttpStatusCode.OK)
        {
            await DisplayAlert("Внимание", "Отчет отправлен.", "Oк");
        }
        else
        {
            await DisplayAlert("Внимание", report.Content, "Oк");
        }
    }

    // Отчет по сотрудникам во всех филиалах
    private async void PersonalReportAllBranchButton_OnClicked(object sender, EventArgs e)
    {
        var report = await OrderModel.GetAllReportPersonal(EmailEntry.Text, DatePickerStart.Date, DatePickerEnd.Date);
        if (report.StatusCode == HttpStatusCode.OK)
        {
            await DisplayAlert("Внимание", "Отчет отправлен!", "Oк");
        }
        else
        {
            await DisplayAlert("Внимание", report.Content, "Oк");
        }
    }
}