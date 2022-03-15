using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Services.Discord;
public interface IDiscordService
{
    public Task InitalizeAsync();
}
