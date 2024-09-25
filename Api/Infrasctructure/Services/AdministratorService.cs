using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Infrasctructure.DB;
using System.Xml.Linq;

namespace minimal_api.Infrasctructure.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly DbContextMinimal _context;
        public AdministratorService(DbContextMinimal context)
        {
            _context = context;
        }

        public Administrator Create(Administrator administrator)
        {
            _context.Administrators.Add(administrator);
            _context.SaveChanges();

            return administrator;
        }

        public Administrator? GetAdministratorById(int id)
        {
            return _context.Administrators.Where(a => a.Id == id).FirstOrDefault(); 
        }

        public List<Administrator> GetAllAdministrators(int? pages)
        {
            var query = _context.Administrators.AsQueryable();
            int itemsPerPage = 10;

            if (pages != null)
            {
                query = query.Skip(((int)pages - 1) * itemsPerPage).Take(itemsPerPage);
            }

            return query.ToList();
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            var qtd = _context.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
            return qtd;
        }

    }
}
