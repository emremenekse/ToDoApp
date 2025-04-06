using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo_App.Domain.Entities;
public class Tag : BaseAuditableEntity
{
    public string? Name { get; set; }
    public string? Color { get; set; }
    public int UsageCount { get; set; }
    public IList<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}
