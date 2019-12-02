
namespace RealApplication01.Web
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Data.Objects.DataClasses;
	using System.Linq;
	using System.ServiceModel.DomainServices.Hosting;
	using System.ServiceModel.DomainServices.Server;


	// The MetadataTypeAttribute identifies BlackEventMetadata as the class
	// that carries additional metadata for the BlackEvent class.
	[MetadataTypeAttribute(typeof(BlackEvent.BlackEventMetadata))]
	public partial class BlackEvent
	{

		// This class allows you to attach custom attributes to properties
		// of the BlackEvent class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class BlackEventMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private BlackEventMetadata()
			{
			}

			public long BlackEventID { get; set; }

			public DateTime Date { get; set; }

			public string Detail { get; set; }

			public Outsider Outsider { get; set; }

			public long OutsiderID { get; set; }

			public string Type { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies ComingMetadata as the class
	// that carries additional metadata for the Coming class.
	[MetadataTypeAttribute(typeof(Coming.ComingMetadata))]
	public partial class Coming
	{

		// This class allows you to attach custom attributes to properties
		// of the Coming class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class ComingMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private ComingMetadata()
			{
			}

			public long ComingID { get; set; }

			public Company Company { get; set; }

			public Nullable<long> CompanyID { get; set; }

			public string ComputerName { get; set; }

			public string ContactElse { get; set; }

			public string ContactEMPID { get; set; }

			public string ContactSectionID { get; set; }

			public string Inspector { get; set; }

			public Nullable<bool> IsDeliver { get; set; }

			public bool IsKickedOut { get; set; }

			[Include()]
			public Outsider Outsider { get; set; }

			public long OutsiderID { get; set; }

			public string PlateNo { get; set; }

			public Nullable<short> TemporaryCardID { get; set; }

			public DateTime TimeIn { get; set; }

			public Nullable<DateTime> TimeOut { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies CompanyMetadata as the class
	// that carries additional metadata for the Company class.
	[MetadataTypeAttribute(typeof(Company.CompanyMetadata))]
	public partial class Company
	{

		// This class allows you to attach custom attributes to properties
		// of the Company class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class CompanyMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private CompanyMetadata()
			{
			}

			public EntityCollection<Coming> Comings { get; set; }

			public long CompanyID { get; set; }

			public EntityCollection<Group> Groups { get; set; }

			public string Name { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies GroupMetadata as the class
	// that carries additional metadata for the Group class.
	[MetadataTypeAttribute(typeof(Group.GroupMetadata))]
	public partial class Group
	{

		// This class allows you to attach custom attributes to properties
		// of the Group class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class GroupMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private GroupMetadata()
			{
			}

			public long AgentID { get; set; }

			public string AgentTelephoneNo { get; set; }

			public Company Company { get; set; }

			public long CompanyID { get; set; }

			public string EmergencyCallNo { get; set; }

			public string EmergencyContact { get; set; }

			public long GroupID { get; set; }

			public Outsider Outsider { get; set; }

			[Include()]
			public EntityCollection<Outsider_Group> Outsider_Group { get; set; }

			public DateTime TimeIn { get; set; }

			public string WorkArea { get; set; }

			public string WorkType { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies IdentifierMetadata as the class
	// that carries additional metadata for the Identifier class.
	[MetadataTypeAttribute(typeof(Identifier.IdentifierMetadata))]
	public partial class Identifier
	{

		// This class allows you to attach custom attributes to properties
		// of the Identifier class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class IdentifierMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private IdentifierMetadata()
			{
			}

			public string Detail { get; set; }

			public bool HaveCopy { get; set; }

			public string IdentifierID { get; set; }

			[Include()]
			public EntityCollection<IdentifierImage> IdentifierImages { get; set; }

			public IdentifierType IdentifierType { get; set; }

			public short IdentifierTypeID { get; set; }

			[Include()]
			public Outsider Outsider { get; set; }

			public long OutsiderID { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies IdentifierImageMetadata as the class
	// that carries additional metadata for the IdentifierImage class.
	[MetadataTypeAttribute(typeof(IdentifierImage.IdentifierImageMetadata))]
	public partial class IdentifierImage
	{

		// This class allows you to attach custom attributes to properties
		// of the IdentifierImage class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class IdentifierImageMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private IdentifierImageMetadata()
			{
			}

			public string FileName { get; set; }

			public Identifier Identifier { get; set; }

			public string IdentifierID { get; set; }

			public short IdentifierTypeID { get; set; }

			public bool IsCropped { get; set; }

			public DateTime SnapDateTime { get; set; }

			public string SpecificType { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies IdentifierTypeMetadata as the class
	// that carries additional metadata for the IdentifierType class.
	[MetadataTypeAttribute(typeof(IdentifierType.IdentifierTypeMetadata))]
	public partial class IdentifierType
	{

		// This class allows you to attach custom attributes to properties
		// of the IdentifierType class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class IdentifierTypeMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private IdentifierTypeMetadata()
			{
			}

			public string Detail { get; set; }

			public EntityCollection<Identifier> Identifiers { get; set; }

			public short IdentifierTypeID { get; set; }

			public short IDLength { get; set; }

			public bool isFixedLength { get; set; }

			public string Name { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies OutsiderMetadata as the class
	// that carries additional metadata for the Outsider class.
	[MetadataTypeAttribute(typeof(Outsider.OutsiderMetadata))]
	public partial class Outsider
	{

		// This class allows you to attach custom attributes to properties
		// of the Outsider class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class OutsiderMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private OutsiderMetadata()
			{
			}

			[Include()]
			public EntityCollection<BlackEvent> BlackEvents { get; set; }

			[Include()]
			public EntityCollection<Coming> Comings { get; set; }

			public EntityCollection<Group> Groups { get; set; }

			[Include()]
			public EntityCollection<Identifier> Identifiers { get; set; }

			public string Name { get; set; }

			public EntityCollection<Outsider_Group> Outsider_Group { get; set; }

			public long OutsiderID { get; set; }

			public string SName { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies Outsider_GroupMetadata as the class
	// that carries additional metadata for the Outsider_Group class.
	[MetadataTypeAttribute(typeof(Outsider_Group.Outsider_GroupMetadata))]
	public partial class Outsider_Group
	{

		// This class allows you to attach custom attributes to properties
		// of the Outsider_Group class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class Outsider_GroupMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private Outsider_GroupMetadata()
			{
			}

			public Group Group { get; set; }

			public long GroupID { get; set; }

			public bool havePhoto { get; set; }

			public bool isPassed { get; set; }

			public Outsider Outsider { get; set; }

			public long OutsiderID { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies v_BlackIdentifierListMetadata as the class
	// that carries additional metadata for the v_BlackIdentifierList class.
	[MetadataTypeAttribute(typeof(v_BlackIdentifierList.v_BlackIdentifierListMetadata))]
	public partial class v_BlackIdentifierList
	{

		// This class allows you to attach custom attributes to properties
		// of the v_BlackIdentifierList class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class v_BlackIdentifierListMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private v_BlackIdentifierListMetadata()
			{
			}

			public string IdentifierID { get; set; }

			public int IdentifierTypeID { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies v_BlackInnerContractorMetadata as the class
	// that carries additional metadata for the v_BlackInnerContractor class.
	[MetadataTypeAttribute(typeof(v_BlackInnerContractor.v_BlackInnerContractorMetadata))]
	public partial class v_BlackInnerContractor
	{

		// This class allows you to attach custom attributes to properties
		// of the v_BlackInnerContractor class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class v_BlackInnerContractorMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private v_BlackInnerContractorMetadata()
			{
			}

			public string CitAdd { get; set; }

			public string CitID { get; set; }

			public Nullable<int> Company { get; set; }

			public Nullable<DateTime> DateofBirth { get; set; }

			public string EMPName { get; set; }

			public string EMPNameTh { get; set; }

			public string EMPSName { get; set; }

			public string EMPSNameTh { get; set; }

			public string FirstSup { get; set; }

			public string Gender { get; set; }

			public int NewID { get; set; }

			public string Telephone { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies v_EmployeeMetadata as the class
	// that carries additional metadata for the v_Employee class.
	[MetadataTypeAttribute(typeof(v_Employee.v_EmployeeMetadata))]
	public partial class v_Employee
	{

		// This class allows you to attach custom attributes to properties
		// of the v_Employee class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class v_EmployeeMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private v_EmployeeMetadata()
			{
			}

			public string EMPID { get; set; }

			public string EMPName { get; set; }

			public string EMPSName { get; set; }

			public string FuncAbbrev { get; set; }

			public string FuncID { get; set; }

			public string FuncName { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies v_SectionMetadata as the class
	// that carries additional metadata for the v_Section class.
	[MetadataTypeAttribute(typeof(v_Section.v_SectionMetadata))]
	public partial class v_Section
	{

		// This class allows you to attach custom attributes to properties
		// of the v_Section class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class v_SectionMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private v_SectionMetadata()
			{
			}

			public string FuncAbbrev { get; set; }

			public string FuncID { get; set; }

			public string FuncName { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies WorkAreaMetadata as the class
	// that carries additional metadata for the WorkArea class.
	[MetadataTypeAttribute(typeof(WorkArea.WorkAreaMetadata))]
	public partial class WorkArea
	{

		// This class allows you to attach custom attributes to properties
		// of the WorkArea class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class WorkAreaMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private WorkAreaMetadata()
			{
			}

			public string Detail { get; set; }

			public string Name { get; set; }

			public long WorkAreaID { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies WorkTypeMetadata as the class
	// that carries additional metadata for the WorkType class.
	[MetadataTypeAttribute(typeof(WorkType.WorkTypeMetadata))]
	public partial class WorkType
	{

		// This class allows you to attach custom attributes to properties
		// of the WorkType class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class WorkTypeMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private WorkTypeMetadata()
			{
			}

			public string Detail { get; set; }

			public string Name { get; set; }

			public long WorkTypeID { get; set; }
		}
	}

	// The MetadataTypeAttribute identifies v_GroupForSelectMetadata as the class
	// that carries additional metadata for the v_GroupForSelect class.
	[MetadataTypeAttribute(typeof(v_GroupForSelect.v_GroupForSelectMetadata))]
	public partial class v_GroupForSelect
	{

		// This class allows you to attach custom attributes to properties
		// of the v_GroupForSelect class.
		//
		// For example, the following marks the Xyz property as a
		// required property and specifies the format for valid values:
		//    [Required]
		//    [RegularExpression("[A-Z][A-Za-z0-9]*")]
		//    [StringLength(32)]
		//    public string Xyz { get; set; }
		internal sealed class v_GroupForSelectMetadata
		{

			// Metadata classes are not meant to be instantiated.
			private v_GroupForSelectMetadata()
			{
			}

			public string AgentName { get; set; }

			public string AgentSName { get; set; }

			public string CompanyName { get; set; }

			public long GroupID { get; set; }

			public Nullable<int> NotPassCount { get; set; }

			public Nullable<int> PassCount { get; set; }

			public DateTime TimeIn { get; set; }

			public Nullable<int> Total { get; set; }
		}
	}
}
