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
using RealApplication01.Web;

namespace RealApplication01
{
	public class Validater
	{
		public static bool sameOutsider(string idA, short idTypeA, string idB, short idTypeB)
		{
			if
				(
					(idA == idB)
					&&
					(
						(idTypeA == idTypeB)
						||
						(idTypeA <= 3 && idTypeB <= 3)
					)
				)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool pass(IdentifierType type, string id)
		{
			return pass(type.IdentifierTypeID, id);
		}

		public static bool pass(int identifierTypeID, string id)
		{
			if (id == "")
				return false;
			
			if (identifierTypeID == 1 || identifierTypeID==2 || identifierTypeID==3)	//3 type of card that have CitizenID
			{
				if (id.Length != 13)
					return false;

				if (id[0] == '0' || id[0] == '9')
					return false;

				try
				{
					int sum = 0;
					for (int i = 0; i <= 11; i++)
					{
						sum+= Int32.Parse(id[i].ToString())*(13-i);
					}
					int lastDigit = (11 - (sum % 11)) % 10;
					if (id[12].ToString() == lastDigit.ToString())
						return true;
					else
						return false;
				}
				catch
				{
					//parse digit error
					return false;
				}
				
			}
			else
			{
				return true;
			}
			//return false;
		}
	}
}
