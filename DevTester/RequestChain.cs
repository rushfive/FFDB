//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using DatStat.Connect.CoreAbstractions.Abstractions.Security;
//using DatStat.Connect.CoreAbstractions.Entities;
//using DatStat.Connect.ServerAbstractions.Services;
//using Microsoft.Extensions.DependencyInjection;

//namespace DatStat.Connect.ServerAbstractions
//{
//	public class GetConditionItemsByActivity : ResultRequestChain<GetConditionItemsByActivity.Context, (ParticipantDataDictionary, ActivityVersion)>
//	{
//		protected override List<Type> NodeTypes { get; } = new List<Type>
//		{
//			typeof(GetParticipantDataDictionary),
//			typeof(GetLatestActivityVersion)
//		};

//		public class Context :
//			GetParticipantDataDictionary.IContext,
//			GetLatestActivityVersion.IContext
//		{
//			public Guid ActivityId { get; }
//			public ParticipantDataDictionary DataDictionary { get; set; }
//			public ActivityVersion Version { get; set; }

//			public Context(Guid activityId)
//			{
//				this.ActivityId = activityId;
//			}
//		}

//		public GetConditionItemsByActivity(IServiceProvider sp) : base(sp)
//		{

//		}

//		protected override Task<(ParticipantDataDictionary, ActivityVersion)> ResolveResult(Context context)
//		{
//			return Task.FromResult((context.DataDictionary, context.Version));
//		}
//	}

//	// STAGES

//	[PopulationItem(PopulationItem.Fields, AccessType.Get)]
//	public class GetParticipantDataDictionary : RequestChainNode<GetConditionItemsByActivity.Context>
//	{
//		public interface IContext
//		{
//			ParticipantDataDictionary DataDictionary { get; set; }
//		}

//		private IParticipantFieldService fieldService { get; }

//		public GetParticipantDataDictionary(IParticipantFieldService fieldService)
//		{
//			this.fieldService = fieldService;
//		}

//		public override async Task ProcessAsync(GetConditionItemsByActivity.Context context)
//		{
//			context.DataDictionary = await this.fieldService.GetDictionaryAsync(false, true);
//		}
//	}

//	[PopulationAdmin]
//	public class GetLatestActivityVersion : RequestChainNode<GetConditionItemsByActivity.Context>
//	{
//		public interface IContext
//		{
//			ActivityVersion Version { get; set; }
//		}

//		private IActivityService activityService { get; }

//		public GetLatestActivityVersion(IActivityService activityService)
//		{
//			this.activityService = activityService;
//		}

//		public override async Task ProcessAsync(GetConditionItemsByActivity.Context context)
//		{
//			context.Version = await this.activityService.GetLatestVersionAsync(context.ActivityId);
//		}
//	}

//	public abstract class ResultRequestChain<TContext, TReturn> : RequestChainBase<TContext>
//	{
//		protected ResultRequestChain(IServiceProvider sp) : base(sp)
//		{

//		}

//		public async Task<TReturn> ProcessAsync(TContext context)
//		{
//			await this.ValidateAccessAsync();

//			foreach (RequestChainNode<TContext> node in this.GetChainNodes())
//			{
//				await node.ProcessAsync(context);
//			}

//			return await this.ResolveResult(context);
//		}

//		protected abstract Task<TReturn> ResolveResult(TContext context);
//	}

//	public abstract class RequestChain<TContext> : RequestChainBase<TContext>
//	{
//		protected RequestChain(IServiceProvider sp) : base(sp)
//		{

//		}

//		public async Task ProcessAsync(TContext context)
//		{
//			// walk through chain, resolve reqiured permissions for the entire thing, and check first


//			foreach (RequestChainNode<TContext> node in this.GetChainNodes())
//			{
//				await node.ProcessAsync(context);
//			}
//		}
//	}

//	public abstract class RequestChainBase<TContext>
//	{
//		protected abstract List<Type> NodeTypes { get; }
//		private IServiceProvider sp { get; }

//		protected RequestChainBase(IServiceProvider sp)
//		{
//			this.sp = sp;
//		}

//		protected List<RequestChainNode<TContext>> GetChainNodes()
//		{
//			var nodes = new List<RequestChainNode<TContext>>();

//			foreach (Type type in this.NodeTypes)
//			{
//				//nodes.Add(
//				//	ActivatorUtilities.CreateInstance<RequestChainNode<TContext>>(this.sp, type));

//				nodes.Add(
//					ActivatorUtilities.CreateInstance(this.sp, type) as RequestChainNode<TContext>);
//			}

//			return nodes;
//		}

//		protected async Task ValidateAccessAsync()
//		{
//			RequestRequiredAccess requiredAccess = this.GetRequiredAccessForRequest();

//			IAccessInfoProvider accessInfoProvider = this.sp.GetRequiredService<IAccessInfoProvider>();
//			IAccessInfo accessInfo = await accessInfoProvider.GetAsync();

//			await requiredAccess.ThrowIfInsufficientAccessAsync(accessInfo);
//		}

//		private RequestRequiredAccess GetRequiredAccessForRequest()
//		{
//			var access = new RequestRequiredAccess();

//			foreach (Type nodeType in this.NodeTypes)
//			{
//				var attributes = nodeType
//					.GetCustomAttributes(typeof(RequestChainAttribute), inherit: false)
//					.Select(a => a as RequestChainAttribute)
//					.ToList();

//				foreach(RequestChainAttribute attr in attributes)
//				{
//					attr.UpdateRequestAccess(access);
//				}
//			}

//			return access;
//		}
//	}

//	public abstract class RequestChainNode<TContext>
//	{
//		public abstract Task ProcessAsync(TContext context);
//	}

//	//public class RequestRequiredAccess
//	//{
//	//	private bool populationAdminRequired { get; set; }

//	//	// Population Items
//	//	private Dictionary<PopulationItem, HashSet<AccessType>> populationItems = new Dictionary<PopulationItem, HashSet<AccessType>>
//	//	{
//	//		{ PopulationItem.Workflows, new HashSet<AccessType>() },
//	//		{ PopulationItem.Campaigns, new HashSet<AccessType>() },
//	//		{ PopulationItem.Activities, new HashSet<AccessType>() },
//	//		{ PopulationItem.Roles, new HashSet<AccessType>() },
//	//		{ PopulationItem.Sites, new HashSet<AccessType>() },
//	//		{ PopulationItem.Fields, new HashSet<AccessType>() },
//	//		{ PopulationItem.UserRoles, new HashSet<AccessType>() },
//	//		{ PopulationItem.EmailFromAddress, new HashSet<AccessType>() },
//	//		{ PopulationItem.Users, new HashSet<AccessType>() },
//	//		{ PopulationItem.UserGroups, new HashSet<AccessType>() }
//	//	};

//	//	private Dictionary<PrivilegedObjectType, HashSet<AccessType>> privilegedObjects = new Dictionary<PrivilegedObjectType, HashSet<AccessType>>
//	//	{
//	//		{ PrivilegedObjectType.ParticipantWorkflows, new HashSet<AccessType>() },
//	//		{ PrivilegedObjectType.Participants, new HashSet<AccessType>() },
//	//		{ PrivilegedObjectType.SubmissionData, new HashSet<AccessType>() },
//	//		{ PrivilegedObjectType.TestParticipantsAndData, new HashSet<AccessType>() }
//	//	};

//	//	public void RequiresPopulationAdmin()
//	//	{
//	//		this.populationAdminRequired = true;
//	//	}

//	//	public void RequiresPopulationItemAccess(PopulationItem item, AccessType access)
//	//	{
//	//		this.populationItems[item].Add(access);
//	//	}

//	//	public void RequiresPrivilegedObjectAccess(PrivilegedObjectType obj, AccessType access)
//	//	{
//	//		this.privilegedObjects[obj].Add(access);
//	//	}

//	//	public async Task ThrowIfInsufficientAccessAsync(IAccessInfo accessInfo)
//	//	{
//	//		var userAccess = accessInfo as IUserAccessInfo;
//	//		bool isUser = userAccess != null;

//	//		if (this.RequiresUser() && !isUser)
//	//		{
//	//			throw new IllegalAccessException("");
//	//		}

//	//		if (isUser)
//	//		{
//	//			bool isPopulationAdmin = await userAccess.IsAPopulationAdminAsync();
//	//			if (!isPopulationAdmin)
//	//			{
//	//				if (this.populationAdminRequired)
//	//				{
//	//					throw new IllegalAccessException("");
//	//				}
//	//			}
//	//			else
//	//			{
//	//				foreach(KeyValuePair<PopulationItem, HashSet<AccessType>> item in this.populationItems)
//	//				{
//	//					foreach(AccessType accessType in item.Value)
//	//					{
//	//						if (!await userAccess.HasPopulationItemAccessAsync(item.Key, accessType))
//	//						{
//	//							throw new IllegalAccessException("");
//	//						}
//	//					}
//	//				}

//	//				foreach (KeyValuePair<PrivilegedObjectType, HashSet<AccessType>> item in this.privilegedObjects)
//	//				{
//	//					foreach (AccessType accessType in item.Value)
//	//					{
//	//						if (!await userAccess.HasObjectAccessAsync(item.Key, accessType))
//	//						{
//	//							throw new IllegalAccessException("");
//	//						}
//	//					}
//	//				}
//	//			}
//	//		}
//	//		else
//	//		{
//	//			// NON user checks
//	//		}
//	//	}

//	//	private bool RequiresUser()
//	//	{
//	//		return this.populationAdminRequired
//	//			|| this.populationItems.Any(kv => kv.Value.Any())
//	//			|| this.privilegedObjects.Any(kv => kv.Value.Any());
//	//	}
//	//}

//	// attributes

//	public abstract class RequestChainAttribute : Attribute
//	{
//		public abstract void UpdateRequestAccess(RequestRequiredAccess access);
//	}

//	public class PopulationItemAttribute : RequestChainAttribute
//	{
//		private PopulationItem item { get; }
//		private AccessType access { get; }

//		public PopulationItemAttribute(PopulationItem item, AccessType access)
//		{
//			this.item = item;
//			this.access = access;
//		}

//		public override void UpdateRequestAccess(RequestRequiredAccess access)
//		{
//			access.RequiresPopulationItemAccess(this.item, this.access);
//		}
//	}

//	public class PopulationAdminAttribute : RequestChainAttribute
//	{
//		public override void UpdateRequestAccess(RequestRequiredAccess access)
//		{
//			access.RequiresPopulationAdmin();
//		}
//	}







//}
