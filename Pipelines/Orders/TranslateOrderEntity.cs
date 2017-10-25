//-----------------------------------------------------------------------
// <copyright file="TranslateOrderEntity.cs" company="Sitecore Corporation">
//     Copyright (c) Sitecore Corporation 1999-2016
// </copyright>
// <summary>Pipeline processor example to translate entity on pipeline</summary>
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
namespace Sitecore.Commerce.Examples.Pipelines.Orders
{
    using System.Linq;

    using Sitecore.Commerce.Connect.CommerceServer.Orders.Models;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Engine.Connect.Pipelines;
    using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Plugin.Orders;

    /// <summary>
    /// The translate order entity.
    /// </summary>
    public class TranslateOrderEntity : TranslateODataEntityToEntity<TranslateOrderToEntityRequest, TranslateOrderToEntityResult, Order, CommerceOrder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateOrderEntity"/> class.
        /// </summary>
        /// <param name="entityFactory">
        /// The entity factory.
        /// </param>
        public TranslateOrderEntity(IEntityFactory entityFactory)
            : base(entityFactory)
        {
        }

        /// <summary>
        /// The translate.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        protected override void Translate(TranslateOrderToEntityRequest request, Order source, CommerceOrder destination)
        {
            base.Translate(request, source, destination);

            if ((source.Components == null) || !source.Components.Any())
            {
                return;
            }

            // Sets all the properties from the custom components to the properties
            foreach (var component in source.Components)
            {
                foreach (var property in component.GetType().GetProperties())
                {
                    var name = component.GetType().Name + "_" + property.Name;
                    if (property.GetValue(component) != null)
                    {
                        destination.Properties[name] = property.GetValue(component);
                    }
                }
            }
        }

        /// <summary>.
        /// Gets the translate destination.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>the destination CommerceConnect entity.</returns>
        protected override CommerceOrder GetTranslateDestination(TranslateOrderToEntityRequest request)
        {
            return this.EntityFactory.Create<CommerceOrder>("Order");
        }
    }
}
