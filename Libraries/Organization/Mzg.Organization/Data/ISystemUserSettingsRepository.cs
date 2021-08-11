using Mzg.Organization.Domain;
using System;

namespace Mzg.Organization.Data
{
    public interface ISystemUserSettingsRepository
    {
        UserSettings FindById(Guid id);
    }
}