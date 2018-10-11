using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PressFitApi.Models;

namespace PressFitApi.Controllers
{
    public class TokenApiController : ApiController
    {
        private PressFitApiContext db = new PressFitApiContext();

        // GET: api/TokenApi
        public IQueryable<Token> GetTokenModels()
        {
            return db.Token;
        }

        // GET: api/TokenApi/5
        [ResponseType(typeof(Token))]
        public IHttpActionResult GetTokenModel(int id)
        {
            Token tokenModel = db.Token.Find(id);
            if (tokenModel == null)
            {
                return NotFound();
            }

            return Ok(tokenModel);
        }

        // PUT: api/TokenApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTokenModel(int id, Token tokenModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tokenModel.Id)
            {
                return BadRequest();
            }

            db.Entry(tokenModel).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TokenModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TokenApi
        [ResponseType(typeof(Token))]
        public IHttpActionResult PostTokenModel(Token tokenModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Token.Add(tokenModel);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tokenModel.Id }, tokenModel);
        }

        // DELETE: api/TokenApi/5
        [ResponseType(typeof(Token))]
        public IHttpActionResult DeleteTokenModel(int id)
        {
            Token tokenModel = db.Token.Find(id);
            if (tokenModel == null)
            {
                return NotFound();
            }

            db.Token.Remove(tokenModel);
            db.SaveChanges();

            return Ok(tokenModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TokenModelExists(int id)
        {
            return db.Token.Count(e => e.Id == id) > 0;
        }
    }
}