//-----------------------------------------------------------------------
// <copyright file="SetCustomFieldsByCommand.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Pipeline processor example to call a custom command that sets a custom field on th cart</summary>
//-----------------------------------------------------------------------
// Copyright 2016 Sitecore Corporation A/S
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file 
// except in compliance with the License. You may obtain a copy of the License at
//       http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the 
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
// either express or implied. See the License for the specific language governing permissions 
// and limitations under the License.
// -------
namespace Sitecore.Commerce.Examples.Pipelines.Cart
{
    using System;

    using Sitecore.Commerce.Engine;
    using Sitecore.Commerce.Engine.Connect;
    using Sitecore.Commerce.Engine.Connect.Pipelines;
    using Sitecore.Commerce.Pipelines;
    using Sitecore.Commerce.Plugin.AdventureWorks.Commands;
    using Sitecore.Commerce.ServiceProxy;
    using Sitecore.Commerce.Services;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Diagnostics;

    /// <summary>
    /// The set custom fields by command.
    /// </summary>
    public class SetCustomFieldsByCommand : PipelineProcessor<ServicePipelineArgs>
    {
        /// <summary>
        /// The process.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public override void Process(ServicePipelineArgs args)
        {
            var request = args.Request as AddCartLinesRequest;
            var result = args.Result as CartResult;
            
            try
            {
                Assert.IsNotNull(request.Cart, "request.Cart");
                Assert.IsNotNullOrEmpty(request.Cart.UserId, "request.Cart.UserId");
                Assert.IsNotNull(request.Lines, "request.Lines");

                // Get the custom property set in the front end code
                var applyVipStatus = Convert.ToBoolean(request.Properties["applyVIPStatus"]);

                var cart = request.Cart;

                // Apply the headers
                var container = this.GetContainer(cart.ShopName, cart.UserId, cart.CustomerId, string.Empty, string.Empty, null);
                var command = Proxy.DoCommand(container.SetCartVIPStatus(cart.ExternalId, applyVipStatus));
                result.HandleCommandMessages(command);
            }
            catch (ArgumentException exception)
            {
                result.Success = false;
                result.SystemMessages.Add(this.CreateSystemMessage(exception));
            }
            catch (AggregateException exception2)
            {
                result.Success = false;
                result.SystemMessages.Add(this.CreateSystemMessage(exception2));
            }
        }

        /// <summary>
        /// The get container.
        /// </summary>
        /// <param name="shopName">
        /// The shop name.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="customerId">
        /// The customer id.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <param name="currency">
        /// The currency.
        /// </param>
        /// <param name="effectiveDate">
        /// The effective date.
        /// </param>
        /// <returns>
        /// The <see cref="Container"/>.
        /// </returns>
        public Container GetContainer(
            string shopName,
            string userId,
            string customerId = "",
            string language = "",
            string currency = "",
            DateTime? effectiveDate = new DateTime?())
        {
            return EngineConnectUtility.GetShopsContainer(
                string.Empty,
                shopName,
                userId,
                customerId,
                language,
                currency,
                effectiveDate);
        }

        /// <summary>
        /// The create system message.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="SystemMessage"/>.
        /// </returns>
        public SystemMessage CreateSystemMessage(Exception ex)
        {
            var message = new SystemMessage
            {
                Message = ex.Message
            };
            if ((ex.InnerException != null) && !ex.Message.Equals(ex.InnerException.Message, StringComparison.OrdinalIgnoreCase))
            {
                message.Message = message.Message + " - " + ex.InnerException.Message;
            }
            return message;
        }


    }
}
