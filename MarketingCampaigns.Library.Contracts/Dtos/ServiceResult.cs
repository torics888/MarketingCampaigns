using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingCampaigns.Library.Contracts.Dtos;
public class ServiceResult<T>
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
}