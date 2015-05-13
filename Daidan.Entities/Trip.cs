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
		public virtual decimal UnitSellingPrice { get; set; }

		public virtual DateTime AddedOn { get; set; }
		public virtual DateTime LastModefied { get; set; }

		public virtual Material Material { set; get; }
		public virtual Site Site { set; get; }
		public virtual Driver Driver { set; get; }
		public virtual Truck Truck { set; get; }
		public virtual Unit Unit { set; get; }

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
