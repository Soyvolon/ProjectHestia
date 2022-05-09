using Microsoft.EntityFrameworkCore;

using ProjectHestia.Data.Database;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHestia.Data.Services.Guild;
public interface IGuildService
{

    public Task<bool> EnsureGuildCreated(ulong guildId);
}
