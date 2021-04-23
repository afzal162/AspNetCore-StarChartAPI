using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var obj = _context.CelestialObjects.Find(id);
            if (obj == null)
                return NotFound();

            var sattelites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id);
            if (sattelites.Count() > 0)
                obj.Satellites = sattelites.ToList();

            return Ok(obj);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var obj = _context.CelestialObjects.Where(x=>x.Name.ToLower() == name.ToLower());
            var list = _context.CelestialObjects.Where(x => x.Name.ToLower() == name.ToLower()).ToList();
            if (list == null || list.Count()==0)
                return NotFound();

            list.ForEach(x =>
            {
                var sattelites = _context.CelestialObjects.Where(y => y.OrbitedObjectId == x.Id);
                if (sattelites.Count() > 0)
                    x.Satellites = sattelites.ToList();
            });
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _context.CelestialObjects.ToList();
            if (list == null)
                return NotFound();

            list.ForEach(x =>
            {
                var sattelites = _context.CelestialObjects.Where(y => y.OrbitedObjectId == x.Id);
                if (sattelites.Count() > 0)
                    x.Satellites = sattelites.ToList();
            });
            return Ok(list);
        }


        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject obj)
        {
            var f = _context.CelestialObjects.Add(obj);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new {id = f.Entity.Id}, obj);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id,  CelestialObject objCele)
        {
            var obj = _context.CelestialObjects.Find(id);
            if (obj == null)
                return NotFound();

            obj.Name = objCele.Name;
            obj.OrbitalPeriod = objCele.OrbitalPeriod;
            obj.OrbitedObjectId = objCele.OrbitedObjectId;

            var f = _context.CelestialObjects.Update(obj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var obj = _context.CelestialObjects.Find(id);
            if (obj == null)
                return NotFound();

            obj.Name = name; 
            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var obj = _context.CelestialObjects.Where(x=>x.Id == id || x.OrbitedObjectId == id);
            if (obj == null || obj.Count() ==0)
                return NotFound();
            _context.CelestialObjects.RemoveRange(obj);
            _context.SaveChanges();
            return NoContent();
        }



    }
}
