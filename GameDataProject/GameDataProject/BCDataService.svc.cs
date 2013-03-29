//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Web;
using System.Web;

namespace GameDataProject
{
    public class BCDataService : DataService<BalboaConstrictorsEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            config.SetEntitySetAccessRule("Games", EntitySetRights.All);
            config.SetEntitySetAccessRule("Players", EntitySetRights.All);
            config.SetEntitySetAccessRule("Events", EntitySetRights.All);
            config.SetEntitySetAccessRule("PlayerDatas", EntitySetRights.All);
            config.SetEntitySetAccessRule("Teams", EntitySetRights.All);
            config.SetEntitySetAccessRule("Attributes", EntitySetRights.All);
            config.SetEntitySetAccessRule("DataTypes", EntitySetRights.All);
            config.SetEntitySetAccessRule("EventAttributes", EntitySetRights.All);
            config.SetEntitySetAccessRule("EventPlayers", EntitySetRights.All);
            config.SetEntitySetAccessRule("EventTypes", EntitySetRights.All);
            config.SetEntitySetAccessRule("GameTeams", EntitySetRights.All);
            config.SetEntitySetAccessRule("Games", EntitySetRights.All);
            //config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }


    }
}
