using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIClientes.Data;
using APIClientes.Models;
using APIClientes.Repository;
using APIClientes.Models.Dto;
using Microsoft.AspNetCore.Authorization;

namespace APIClientes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // ESTO ES LO QUE HACE QUE UN USUARIO PUEDA ACCEDER A LOS METODOS PARA GESTIONAR INFORMACION
    public class ClientesController : ControllerBase
    {
        // HACEMOS UNA INSTANCIA DE NUESTRA INTERFAZ PARA TRAER LOS METODOS DESDE EL REPOSITORIO
        // APLICANDO NUESTRO REPOSITORIOS EN NUESTROS CONTROLADORES BUENA PRACTICA !!!!
        private readonly IClientesRepository _clienteRepository;

        // LA CLASE RESPONSE SE VA A ENCARGAR DE MOSTAR TODA LA RESPUESTA QUE NOSOTROS OBTENGAMOS
        // DE NUESTRO REPOSITORIO Y DE NUESTROS DATOS
        protected ResponseDto _response;

        // private readonly ApplicationDbContext _context; MALA PRACTICA !!!!!!!

        // public ClientesController(ApplicationDbContext context) MALA PRACTICA !!!
        public ClientesController(IClientesRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
            _response = new ResponseDto();
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            // return await _context.Clientes.ToListAsync(); MALA PRACTICA !!!

            // UTILIZAMOS UN TRY CATCH
            try
            {
                var listaClientes = await _clienteRepository.GetClientes();
                _response.Result = listaClientes;
                _response.DisplayMessage = "Lista de Clientes";

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }

            return Ok(_response);
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            /*var cliente = await _context.Clientes.FindAsync(id);  MALA PRACTICA !!!

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;*/

            // VALIDO SI EL ID EXISTE
            var cliente = await _clienteRepository.GetClienteById(id);
            if (cliente == null)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Cliente No Existe";
                return NotFound(_response);
            }

            _response.Result = cliente;
            _response.DisplayMessage = "Información del Cliente";
            return Ok(_response);
        }

        // PUT: api/Clientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, ClienteDto clienteDto)// Cambiamos Cliente por ClienteDTO
        {
            // UTILIZAMOS TRY CATCH
            try
            {
                ClienteDto model = await _clienteRepository.CreateUpdate(clienteDto);
                _response.Result = model;
                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.IsSuccess=false;
                _response.DisplayMessage = "Error al Actualizar el Registro";
                _response.ErrorMessage = new List<string> { ex.ToString() };
                return BadRequest(_response);
            }

            /*if (id != cliente.Id)
            {
                return BadRequest();
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();*/


        }

        // POST: api/Clientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(ClienteDto clienteDto) // Cambiamos Cliente por ClienteDTO
        {
            /*_context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCliente", new { id = cliente.Id }, cliente);*/

            // UTILIZAMOS UN TRY CATCH
            try
            {
                ClienteDto model = await _clienteRepository.CreateUpdate(clienteDto);
                _response.Result = model;
                return CreatedAtAction("GetCliente", new { id = model.Id }, _response);
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.DisplayMessage = "Error al Grabar el Registro";
                _response.ErrorMessage = new List<string> { ex.ToString() };
                return BadRequest(_response);
            }

        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            /*var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();*/

            // UTILIZAMOS TRY CATCH
            try
            {
                bool clienteEliminado = await _clienteRepository.DeleteCliente(id);
                if (clienteEliminado)
                {
                    _response.Result = clienteEliminado;
                    _response.DisplayMessage = "Cliente Eliminado con Exito";
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.DisplayMessage = "Error al Eliminar el Cliente";
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
                return BadRequest(_response);
            }
        }

        private bool ClienteExists(int id)
        {
            // return _context.Clientes.Any(e => e.Id == id);
            var cliente = _clienteRepository.GetClienteById(id);
            if (cliente == null)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Cliente No Existe";
                return false;
            }

            _response.Result = cliente;
            _response.DisplayMessage = "Información del Cliente";
            return true;
        }
    }
}
