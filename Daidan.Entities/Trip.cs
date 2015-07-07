using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
    public class Trip
    {
		public virtual long Id { get; set; }
		public virtual DateTime Date { get; set; }
		public virtual string VoucherNumber { get; set; }
		public virtual string PONumber { get; set; }
		public virtual string TicketNumber { get; set; }

		public virtual decimal UnitCost { get; set; }
		public virtual decimal UnitsQuantity { get; set; }
		public virtual decimal ExtraCost { get; set; }
		public virtual decimal UnitSellingPrice  { get; set; }
		public virtual decimal AdministrationPercentage { get; set; }

		public virtual DateTime AddedOn { get; set; }
		public virtual DateTime? LastModifiedOn { get; set; }

		public virtual Material Material { set; get; }
		public virtual Site Site { set; get; }
		public virtual Driver Driver { set; get; }
		public virtual Truck Truck { set; get; }
		public virtual Unit Unit { set; get; }

		public virtual SystemAdmin AddedBy { set; get; }
		public virtual SystemAdmin LastModefiedBy { set; get; }

		public virtual decimal TripCost
		{
			get
			{
				return this.UnitsQuantity * this.UnitCost;
			}
		}

		public virtual decimal TripTotalCost
		{
			get
			{
				return this.TripCost + this.ExtraCost;
			}
		}

		public virtual decimal TripTotalPrice
		{
			get
			{
				return this.UnitSellingPrice * this.UnitsQuantity;
			}
		}

		public virtual decimal TripGrossProfit
		{
			get
			{
				if (this.TripTotalPrice > 0)
				{
					return this.TripTotalPrice - this.TripTotalCost;
				}
				else
					return 0;
			}
		}

		public virtual decimal TripNetProfit
		{
			get
			{
				if (this.TripGrossProfit != 0)
				{
					if (this.AdministrationPercentage > 0)
						return this.TripGrossProfit * (1 - (this.AdministrationPercentage / 100));
					else
						return this.TripGrossProfit;
				}
				else
					return 0;
			}
		}

		public virtual decimal AdminFeesAmount
		{
			get
			{
				if (this.AdministrationPercentage > 0 && this.TripGrossProfit > 0)
					return this.TripGrossProfit * this.AdministrationPercentage / 100;
				else
					return 0;
			}
		}

		public virtual string QuantityCaption
		{
			get
			{
				string x = string.Format("{0} {1}", this.UnitsQuantity, this.Unit.Name);
				return x;
			}
		}
    }
}
