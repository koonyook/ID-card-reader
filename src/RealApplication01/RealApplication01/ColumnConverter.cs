using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using RealApplication01.Web;
using System.ServiceModel.DomainServices.Client;
using System.Linq;
using System.Collections.Generic;
using RealApplication01;
using System.Windows.Media.Imaging;

namespace Converter
{
	public class BlackEventsToCountConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (value as EntityCollection<BlackEvent>).Count;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ComingsToCountConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (value as EntityCollection<Coming>).Count;		//return only zero now
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class IdentifiersToFirstIdentifierIDConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Identifier iden = (value as EntityCollection<Identifier>).FirstOrDefault<Identifier>(item => item.IdentifierTypeID<=3);
			if (iden != null)
				return iden.IdentifierID;
			else
			{
				iden = iden = (value as EntityCollection<Identifier>).FirstOrDefault<Identifier>();
				if (iden != null)
					return iden.IdentifierID;
				else
					return "no identifier";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// ///////////////////////for coming history table///////////////////////////////////
	/// </summary>
	public class CompanyIDToNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//cannot do like this because domaindatasource not load company now
			//return (value as Company).Name;

			//use myvar for save bandwidth
			if (value == null)
				return "-";

			foreach (Company comp in myvar.companyList)
			{
				long x=(long)value;
				if (comp.CompanyID == x)
					return comp.Name;
			}

			return "-";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class EmpIDToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//cannot do like this because domaindatasource not load company now
			//return (value as Company).Name;

			//use myvar for save bandwidth
			if (value == null)
				return "-";

			foreach (v_Employee emp in myvar.employeeList)
			{
				string x = (string)value;
				if (emp.EMPID == x)
					return emp.EMPName+" "+emp.EMPSName+" ("+emp.FuncAbbrev+")";
			}

			return "-";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class SectionIDToNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//cannot do like this because domaindatasource not load company now
			//return (value as Company).Name;

			//use myvar for save bandwidth
			if (value == null)
				return "-";

			foreach (v_Section sec in myvar.sectionList)
			{
				string x = (string)value;
				if (sec.FuncID == x)
					return sec.FuncAbbrev;			//edit here to make it fullname
			}

			return "-";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	////////////////////////////////////////////////////////////

	public class OutsiderToComingsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return new List<Coming>();
			return (value as Outsider).Comings;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class OutsiderToIdentifiersConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return new List<Identifier>();
			return (value as Outsider).Identifiers;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class OutsiderToBlackEventsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return new List<Coming>();
			return (value as Outsider).BlackEvents;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class IdentifierTypeIDToNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			//cannot do like this because domaindatasource not load company now
			//return (value as Company).Name;

			//use myvar for save bandwidth
			if (value == null)
				return "-";

			foreach (IdentifierType type in myvar.identifierTypeList)
			{
				short x = (short)value;
				if (type.IdentifierTypeID == x)
					return type.Name;			//edit here to make it fullname
			}

			return "-";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// not use now, use sort in query instead
	/// </summary>
	public class IdentifiersImagesToLastIdentifierImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			
			IdentifierImage dummy;
			if (value != null)
			{
				dummy = (value as IEnumerable<IdentifierImage>).LastOrDefault<IdentifierImage>();
			}
			else
			{
				dummy = null;
			}

			if (dummy == null)
			{
				dummy = new IdentifierImage();
				
				dummy.FileName = "NoImage.jpg";
			}
			return dummy;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class FileNameToImageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null || (string)value == "")
			{
				return new BitmapImage(new Uri(App.Current.Host.Source, "../images/"
					+ "NoImage.jpg")); ;
			}
			else
			{
				return new BitmapImage(new Uri(App.Current.Host.Source, "../images/"
					+ (string)value));
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class IdentifierToIdentifierTypeIDConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return (short)(-1);
			}
			else
			{
				return (value as Identifier).IdentifierTypeID;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class IdentifierToIdentifierIDConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return (short)(-1);
			}
			else
			{
				return (value as Identifier).IdentifierID;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
