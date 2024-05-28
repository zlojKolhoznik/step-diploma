using System.ComponentModel.DataAnnotations;

namespace Olx.Models;

public enum OrderState
{
    [Display(Name = "Очікується оплата")]
    Pending,
    [Display(Name = "Очікується відправка")]
    Processing,
    [Display(Name = "Відправлено")]
    InDelivery,
    [Display(Name = "Доставлено")]
    InWarehouse,
    [Display(Name = "Завершено")]
    Completed,
    [Display(Name = "Скасовано")]
    Canceled
}