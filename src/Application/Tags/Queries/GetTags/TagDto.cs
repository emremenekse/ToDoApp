﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo_App.Application.Common.Mappings;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.Tags.Queries.GetTags;
public class TagDto : IMapFrom<Tag>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public int UsageCount { get; set; }
}
