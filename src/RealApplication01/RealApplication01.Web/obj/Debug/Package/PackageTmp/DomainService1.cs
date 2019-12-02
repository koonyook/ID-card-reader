
namespace RealApplication01.Web
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Data;
	using System.Linq;
	using System.Linq.Dynamic;
	using System.ServiceModel.DomainServices.EntityFramework;
	using System.ServiceModel.DomainServices.Hosting;
	using System.ServiceModel.DomainServices.Server;


	// Implements application logic using the OutsiderDB1Entities context.
	// TODO: Add your application logic to these methods or in additional methods.
	// TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
	// Also consider adding roles to restrict access as appropriate.
	// [RequiresAuthentication]
	[EnableClientAccess()]
	public class DomainService1 : LinqToEntitiesDomainService<OutsiderDB1Entities>
	{

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'BlackEvents' query.
		[Query(IsDefault = true)]
		public IQueryable<BlackEvent> GetBlackEvents()
		{
			return this.ObjectContext.BlackEvents;
		}

		public void InsertBlackEvent(BlackEvent blackEvent)
		{
			if ((blackEvent.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(blackEvent, EntityState.Added);
			}
			else
			{
				this.ObjectContext.BlackEvents.AddObject(blackEvent);
			}
		}

		public void UpdateBlackEvent(BlackEvent currentBlackEvent)
		{
			this.ObjectContext.BlackEvents.AttachAsModified(currentBlackEvent, this.ChangeSet.GetOriginal(currentBlackEvent));
		}

		public void DeleteBlackEvent(BlackEvent blackEvent)
		{
			if ((blackEvent.EntityState == EntityState.Detached))
			{
				this.ObjectContext.BlackEvents.Attach(blackEvent);
			}
			this.ObjectContext.BlackEvents.DeleteObject(blackEvent);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Comings' query.
		[Query(IsDefault = true)]
		public IQueryable<Coming> GetComings()
		{
			return this.ObjectContext.Comings;
		}

		[Query]
		public IQueryable<Coming> GetComingsForCheckOutWithOutsider()
		{
			return this.ObjectContext.Comings.Include("Outsider").Where(item =>
				//item.TimeIn>DateTime.Today &&			//Try to get today only
				item.TimeOut == null
				);
		}

		public void InsertComing(Coming coming)
		{
			if ((coming.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(coming, EntityState.Added);
			}
			else
			{
				this.ObjectContext.Comings.AddObject(coming);
			}
		}

		public void UpdateComing(Coming currentComing)
		{
			this.ObjectContext.Comings.AttachAsModified(currentComing, this.ChangeSet.GetOriginal(currentComing));
		}

		public void DeleteComing(Coming coming)
		{
			if ((coming.EntityState == EntityState.Detached))
			{
				this.ObjectContext.Comings.Attach(coming);
			}
			this.ObjectContext.Comings.DeleteObject(coming);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Companies' query.
		[Query(IsDefault = true)]
		public IQueryable<Company> GetCompanies()
		{
			return this.ObjectContext.Companies.OrderBy(item => item.Name);
		}

		public void InsertCompany(Company company)
		{
			if ((company.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(company, EntityState.Added);
			}
			else
			{
				this.ObjectContext.Companies.AddObject(company);
			}
		}

		public void UpdateCompany(Company currentCompany)
		{
			this.ObjectContext.Companies.AttachAsModified(currentCompany, this.ChangeSet.GetOriginal(currentCompany));
		}

		public void DeleteCompany(Company company)
		{
			if ((company.EntityState == EntityState.Detached))
			{
				this.ObjectContext.Companies.Attach(company);
			}
			this.ObjectContext.Companies.DeleteObject(company);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Groups' query.
		[Query(IsDefault = true)]
		public IQueryable<Group> GetGroups()
		{
			return this.ObjectContext.Groups;
		}

		[Query]
		public IQueryable<Group> GetGroupsWithOutsider_Groups(long groupID)
		{
			return this.ObjectContext.Groups.Include("Outsider_Group").Where(item => item.GroupID==groupID);
		}

		public void InsertGroup(Group group)
		{
			if ((group.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(group, EntityState.Added);
			}
			else
			{
				this.ObjectContext.Groups.AddObject(group);
			}
		}

		public void UpdateGroup(Group currentGroup)
		{
			this.ObjectContext.Groups.AttachAsModified(currentGroup, this.ChangeSet.GetOriginal(currentGroup));
		}

		public void DeleteGroup(Group group)
		{
			if ((group.EntityState == EntityState.Detached))
			{
				this.ObjectContext.Groups.Attach(group);
			}
			this.ObjectContext.Groups.DeleteObject(group);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Identifiers' query.
		[Query(IsDefault = true)]
		public IQueryable<Identifier> GetIdentifiers()
		{
			return this.ObjectContext.Identifiers;
		}

		/// <summary>
		/// use before add new blacklist
		/// </summary>
		/// <param name="identifierID"></param>
		/// <returns></returns>
		public IQueryable<Identifier> GetIdentifiersWithOutsidersFromIdentifierID(string identifierID)
		{
			return this.ObjectContext.Identifiers.Include("Outsider").Where(item => item.IdentifierID==identifierID);
		}

		[Query]
		public IQueryable<Identifier> GetIdentifiersWithOutsidersAndImages()
		{
			return this.ObjectContext.Identifiers.Include("Outsider").Include("IdentifierImages");
		}

		public void InsertIdentifier(Identifier identifier)
		{
			if ((identifier.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(identifier, EntityState.Added);
			}
			else
			{
				this.ObjectContext.Identifiers.AddObject(identifier);
			}
		}

		public void UpdateIdentifier(Identifier currentIdentifier)
		{
			this.ObjectContext.Identifiers.AttachAsModified(currentIdentifier, this.ChangeSet.GetOriginal(currentIdentifier));
		}

		public void DeleteIdentifier(Identifier identifier)
		{
			if ((identifier.EntityState == EntityState.Detached))
			{
				this.ObjectContext.Identifiers.Attach(identifier);
			}
			this.ObjectContext.Identifiers.DeleteObject(identifier);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'IdentifierImages' query.
		[Query(IsDefault = true)]
		public IQueryable<IdentifierImage> GetIdentifierImages()
		{
			return this.ObjectContext.IdentifierImages;
		}

		/// <summary>
		/// should OrderBy for show the lastest snapshot
		/// </summary>
		/// <param name="identifierID"></param>
		/// <param name="identifierTypeID"></param>
		/// <returns></returns>
		public IQueryable<IdentifierImage> GetIdentifierImagesFromIdentifier(string identifierID,short identifierTypeID)
		{
			return this.ObjectContext.IdentifierImages.Where<IdentifierImage>(item =>
				item.IdentifierID == identifierID && item.IdentifierTypeID==identifierTypeID
				).OrderByDescending(item => item.SnapDateTime);
		}

		public void InsertIdentifierImage(IdentifierImage identifierImage)
		{
			if ((identifierImage.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(identifierImage, EntityState.Added);
			}
			else
			{
				this.ObjectContext.IdentifierImages.AddObject(identifierImage);
			}
		}

		public void UpdateIdentifierImage(IdentifierImage currentIdentifierImage)
		{
			this.ObjectContext.IdentifierImages.AttachAsModified(currentIdentifierImage, this.ChangeSet.GetOriginal(currentIdentifierImage));
		}

		public void DeleteIdentifierImage(IdentifierImage identifierImage)
		{
			if ((identifierImage.EntityState == EntityState.Detached))
			{
				this.ObjectContext.IdentifierImages.Attach(identifierImage);
			}
			this.ObjectContext.IdentifierImages.DeleteObject(identifierImage);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'IdentifierTypes' query.
		[Query(IsDefault = true)]
		public IQueryable<IdentifierType> GetIdentifierTypes()
		{
			return this.ObjectContext.IdentifierTypes;
		}

		public void InsertIdentifierType(IdentifierType identifierType)
		{
			if ((identifierType.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(identifierType, EntityState.Added);
			}
			else
			{
				this.ObjectContext.IdentifierTypes.AddObject(identifierType);
			}
		}

		public void UpdateIdentifierType(IdentifierType currentIdentifierType)
		{
			this.ObjectContext.IdentifierTypes.AttachAsModified(currentIdentifierType, this.ChangeSet.GetOriginal(currentIdentifierType));
		}

		public void DeleteIdentifierType(IdentifierType identifierType)
		{
			if ((identifierType.EntityState == EntityState.Detached))
			{
				this.ObjectContext.IdentifierTypes.Attach(identifierType);
			}
			this.ObjectContext.IdentifierTypes.DeleteObject(identifierType);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Outsiders' query.
		[Query(IsDefault = true)]
		public IQueryable<Outsider> GetOutsiders()
		{
			return this.ObjectContext.Outsiders;
		}
		
		public IQueryable<Outsider> GetOutsiders_BlackEvents_Identifiers_Comings()
		{
			return this.ObjectContext.Outsiders.Include("Comings").Include("BlackEvents").Include("Identifiers");
		}

		public IQueryable<Outsider> GetOutsiders_BlackEvents_Identifiers_Comings_Filter(string searcher)
		{
			return this.ObjectContext.Outsiders.Include("Comings").Include("BlackEvents").Include("Identifiers").Where(item => 
				item.Name.Contains(searcher) || item.SName.Contains(searcher) || 
				(item.Identifiers.Count != 0 && (item.Identifiers.Where<Identifier>(
					iden => (iden.IdentifierID.Contains(searcher)))).Count<Identifier>() > 0 )
			);
		}

		public IQueryable<Outsider> GetOutsiders_BlackEvents_Identifiers_Comings_FullFilter(bool blackListOnly,bool todayOnly,string searcher)
		{
			return this.ObjectContext.Outsiders.Include("Comings").Include("BlackEvents").Include("Identifiers").Where(item =>
				(
					(!todayOnly) || ( item.Comings.Where<Coming>(ma => ma.TimeIn > DateTime.Today).Count<Coming>() > 0 )
				)
				&&
				(
					(!blackListOnly) || (item.BlackEvents.Count<BlackEvent>() > 0)
				)
				&&
				(
					item.Name.Contains(searcher) || item.SName.Contains(searcher) ||
					(item.Identifiers.Count != 0 && (item.Identifiers.Where<Identifier>(
						iden => (iden.IdentifierID.Contains(searcher)))).Count<Identifier>() > 0)
				)
			);
		}

		public void InsertOutsider(Outsider outsider)
		{
			if ((outsider.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(outsider, EntityState.Added);
			}
			else
			{
				this.ObjectContext.Outsiders.AddObject(outsider);
			}
		}

		public void UpdateOutsider(Outsider currentOutsider)
		{
			this.ObjectContext.Outsiders.AttachAsModified(currentOutsider, this.ChangeSet.GetOriginal(currentOutsider));
		}

		public void DeleteOutsider(Outsider outsider)
		{
			if ((outsider.EntityState == EntityState.Detached))
			{
				this.ObjectContext.Outsiders.Attach(outsider);
			}
			this.ObjectContext.Outsiders.DeleteObject(outsider);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Outsider_Group' query.
		[Query(IsDefault = true)]
		public IQueryable<Outsider_Group> GetOutsider_Group()
		{
			return this.ObjectContext.Outsider_Group;	//donot change order of this query
		}

		public void InsertOutsider_Group(Outsider_Group outsider_Group)
		{
			if ((outsider_Group.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(outsider_Group, EntityState.Added);
			}
			else
			{
				this.ObjectContext.Outsider_Group.AddObject(outsider_Group);
			}
		}

		public void UpdateOutsider_Group(Outsider_Group currentOutsider_Group)
		{
			this.ObjectContext.Outsider_Group.AttachAsModified(currentOutsider_Group, this.ChangeSet.GetOriginal(currentOutsider_Group));
		}

		public void DeleteOutsider_Group(Outsider_Group outsider_Group)
		{
			if ((outsider_Group.EntityState == EntityState.Detached))
			{
				this.ObjectContext.Outsider_Group.Attach(outsider_Group);
			}
			this.ObjectContext.Outsider_Group.DeleteObject(outsider_Group);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'v_BlackIdentifierList' query.
		[Query(IsDefault = true)]
		public IQueryable<v_BlackIdentifierList> GetV_BlackIdentifierList()
		{
			return this.ObjectContext.v_BlackIdentifierList;
		}

		public void InsertV_BlackIdentifierList(v_BlackIdentifierList v_BlackIdentifierList)
		{
			if ((v_BlackIdentifierList.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(v_BlackIdentifierList, EntityState.Added);
			}
			else
			{
				this.ObjectContext.v_BlackIdentifierList.AddObject(v_BlackIdentifierList);
			}
		}

		public void UpdateV_BlackIdentifierList(v_BlackIdentifierList currentv_BlackIdentifierList)
		{
			this.ObjectContext.v_BlackIdentifierList.AttachAsModified(currentv_BlackIdentifierList, this.ChangeSet.GetOriginal(currentv_BlackIdentifierList));
		}

		public void DeleteV_BlackIdentifierList(v_BlackIdentifierList v_BlackIdentifierList)
		{
			if ((v_BlackIdentifierList.EntityState == EntityState.Detached))
			{
				this.ObjectContext.v_BlackIdentifierList.Attach(v_BlackIdentifierList);
			}
			this.ObjectContext.v_BlackIdentifierList.DeleteObject(v_BlackIdentifierList);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'v_BlackInnerContractor' query.
		[Query(IsDefault = true)]
		public IQueryable<v_BlackInnerContractor> GetV_BlackInnerContractor()
		{
			return this.ObjectContext.v_BlackInnerContractor;
		}

		public void InsertV_BlackInnerContractor(v_BlackInnerContractor v_BlackInnerContractor)
		{
			if ((v_BlackInnerContractor.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(v_BlackInnerContractor, EntityState.Added);
			}
			else
			{
				this.ObjectContext.v_BlackInnerContractor.AddObject(v_BlackInnerContractor);
			}
		}

		public void UpdateV_BlackInnerContractor(v_BlackInnerContractor currentv_BlackInnerContractor)
		{
			this.ObjectContext.v_BlackInnerContractor.AttachAsModified(currentv_BlackInnerContractor, this.ChangeSet.GetOriginal(currentv_BlackInnerContractor));
		}

		public void DeleteV_BlackInnerContractor(v_BlackInnerContractor v_BlackInnerContractor)
		{
			if ((v_BlackInnerContractor.EntityState == EntityState.Detached))
			{
				this.ObjectContext.v_BlackInnerContractor.Attach(v_BlackInnerContractor);
			}
			this.ObjectContext.v_BlackInnerContractor.DeleteObject(v_BlackInnerContractor);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'v_Employee' query.
		[Query(IsDefault = true)]
		public IQueryable<v_Employee> GetV_Employee()
		{
			return this.ObjectContext.v_Employee.OrderBy(item => item.EMPName);
		}

		public void InsertV_Employee(v_Employee v_Employee)
		{
			if ((v_Employee.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(v_Employee, EntityState.Added);
			}
			else
			{
				this.ObjectContext.v_Employee.AddObject(v_Employee);
			}
		}

		public void UpdateV_Employee(v_Employee currentv_Employee)
		{
			this.ObjectContext.v_Employee.AttachAsModified(currentv_Employee, this.ChangeSet.GetOriginal(currentv_Employee));
		}

		public void DeleteV_Employee(v_Employee v_Employee)
		{
			if ((v_Employee.EntityState == EntityState.Detached))
			{
				this.ObjectContext.v_Employee.Attach(v_Employee);
			}
			this.ObjectContext.v_Employee.DeleteObject(v_Employee);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'v_Section' query.
		[Query(IsDefault = true)]
		public IQueryable<v_Section> GetV_Section()
		{
			return this.ObjectContext.v_Section.OrderBy(item => item.FuncName);
		}

		public void InsertV_Section(v_Section v_Section)
		{
			if ((v_Section.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(v_Section, EntityState.Added);
			}
			else
			{
				this.ObjectContext.v_Section.AddObject(v_Section);
			}
		}

		public void UpdateV_Section(v_Section currentv_Section)
		{
			this.ObjectContext.v_Section.AttachAsModified(currentv_Section, this.ChangeSet.GetOriginal(currentv_Section));
		}

		public void DeleteV_Section(v_Section v_Section)
		{
			if ((v_Section.EntityState == EntityState.Detached))
			{
				this.ObjectContext.v_Section.Attach(v_Section);
			}
			this.ObjectContext.v_Section.DeleteObject(v_Section);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'WorkAreas' query.
		[Query(IsDefault = true)]
		public IQueryable<WorkArea> GetWorkAreas()
		{
			return this.ObjectContext.WorkAreas;
		}

		public void InsertWorkArea(WorkArea workArea)
		{
			if ((workArea.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(workArea, EntityState.Added);
			}
			else
			{
				this.ObjectContext.WorkAreas.AddObject(workArea);
			}
		}

		public void UpdateWorkArea(WorkArea currentWorkArea)
		{
			this.ObjectContext.WorkAreas.AttachAsModified(currentWorkArea, this.ChangeSet.GetOriginal(currentWorkArea));
		}

		public void DeleteWorkArea(WorkArea workArea)
		{
			if ((workArea.EntityState == EntityState.Detached))
			{
				this.ObjectContext.WorkAreas.Attach(workArea);
			}
			this.ObjectContext.WorkAreas.DeleteObject(workArea);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'WorkTypes' query.
		[Query(IsDefault = true)]
		public IQueryable<WorkType> GetWorkTypes()
		{
			return this.ObjectContext.WorkTypes;
		}

		public void InsertWorkType(WorkType workType)
		{
			if ((workType.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(workType, EntityState.Added);
			}
			else
			{
				this.ObjectContext.WorkTypes.AddObject(workType);
			}
		}

		public void UpdateWorkType(WorkType currentWorkType)
		{
			this.ObjectContext.WorkTypes.AttachAsModified(currentWorkType, this.ChangeSet.GetOriginal(currentWorkType));
		}

		public void DeleteWorkType(WorkType workType)
		{
			if ((workType.EntityState == EntityState.Detached))
			{
				this.ObjectContext.WorkTypes.Attach(workType);
			}
			this.ObjectContext.WorkTypes.DeleteObject(workType);
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'v_GroupForSelect' query.
		[Query(IsDefault = true)]
		public IQueryable<v_GroupForSelect> GetV_GroupForSelect()
		{
			return this.ObjectContext.v_GroupForSelect.OrderByDescending(item => item.GroupID);
		}

		public IQueryable<v_GroupForSelect> GetV_GroupForSelectWithFilter(string searcher)
		{
			return this.ObjectContext.v_GroupForSelect.Where(item => 
					item.AgentName.Contains(searcher)
					|| item.AgentSName.Contains(searcher)
					|| item.CompanyName.Contains(searcher)
					)
				.OrderByDescending(item => item.GroupID);
		}

		public void InsertV_GroupForSelect(v_GroupForSelect v_GroupForSelect)
		{
			if ((v_GroupForSelect.EntityState != EntityState.Detached))
			{
				this.ObjectContext.ObjectStateManager.ChangeObjectState(v_GroupForSelect, EntityState.Added);
			}
			else
			{
				this.ObjectContext.v_GroupForSelect.AddObject(v_GroupForSelect);
			}
		}

		public void UpdateV_GroupForSelect(v_GroupForSelect currentv_GroupForSelect)
		{
			this.ObjectContext.v_GroupForSelect.AttachAsModified(currentv_GroupForSelect, this.ChangeSet.GetOriginal(currentv_GroupForSelect));
		}

		public void DeleteV_GroupForSelect(v_GroupForSelect v_GroupForSelect)
		{
			if ((v_GroupForSelect.EntityState == EntityState.Detached))
			{
				this.ObjectContext.v_GroupForSelect.Attach(v_GroupForSelect);
			}
			this.ObjectContext.v_GroupForSelect.DeleteObject(v_GroupForSelect);
		}
	}
}


