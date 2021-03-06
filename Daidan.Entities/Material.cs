﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daidan.Entities
{
	public class Material
	{
		public virtual int Id { get; set; }
		[Required(ErrorMessage = "Please enter material name")]
		public virtual string Name { get; set; }
		public virtual bool IsActive { get; set; }
	}
}
