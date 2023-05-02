using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Trilhas.Data.Enums;
using Trilhas.Models.Enums;

namespace Trilhas.Controllers
{
    public class EnumController : DefaultController
    {
        public EnumController(UserManager<IdentityUser> userManager) : base(userManager)
        {
        }

        [HttpGet]
        public IActionResult RecuperarEnumModalidade()
        {
            var lista = new List<EnumViewModel>();

            foreach (var value in Enum.GetValues(typeof(EnumModalidade)))
            {
                lista.Add(new EnumViewModel
                {
                    Codigo = (int)value,
                    Nome = value.ToString()
                });
            }

            return Json(lista);
        }
    }
}
