using R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Models;
using R5.FFDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.FFDB.Components.CoreData.Dynamic.Rosters.Sources.V1.Mappers
{
	public class ToCoreDataMapper : IMapper<RosterVersionedModel, Roster>
	{
		public Roster Map(RosterVersionedModel versionedModel)
		{
			return RosterVersionedModel.ToCoreEntity(versionedModel);
		}
	}
}
