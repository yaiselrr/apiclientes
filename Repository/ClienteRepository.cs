using APIClientes.Data;
using APIClientes.Models;
using APIClientes.Models.Dto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIClientes.Repository
{
    // HACEMOS EXTENDER LA CLASE DE ICLIENTEREPOSITORY
    public class ClienteRepository : IClientesRepository
    {
        // CREAMOS LAS VARIABLES PARA LA CONEXION CON BD Y EL MAPEO Y LAS INICIALIZAMOS EN EL CONTRUCTOR
        // VARIABLE PARA LA CONEXION BASE DE DATOS
        private readonly ApplicationDbContext _dbContext;

        // VARIABLE PARA EL MAPEO
        private IMapper _mapper;
        public ClienteRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<ClienteDto> CreateUpdate(ClienteDto clienteDto)
        {
            // VAMOS A MAPEAR PORQUE ESTAMOS RECIBIENDO UN CLIENTEDTO para llevarlo a CLIENTE
            Cliente cliente = _mapper.Map<ClienteDto, Cliente>(clienteDto);

            // VERIFICAMOS SI ES UNA ACTUALIZACION
            if (cliente.Id > 0) // SI ES MAYOR QUE CERO ES UNA ACTUALIZACION
            { 
                // ENVIAMOS TODO EL OBJETO CLIENTE
                _dbContext.Clientes.Update(cliente);
            }
            else
            {
                // EN CASO CONTRARIO SE TRATA DE UNA CREACION DE UN NUEVO REGISTRO
                await _dbContext.Clientes.AddAsync(cliente);
            } 
            // GRABAMOS LOS CAMBIOS
            await _dbContext.SaveChangesAsync();

            // HACEMOS EL RETONO DE TIPO CLIENTE DTO MAPENADO
            return _mapper.Map<Cliente, ClienteDto>(cliente);



        }

        public async Task<bool> DeleteCliente(int id)
        {
            // HACEMOS EL CONTROL A TRAVES DE UN TRY CATCH
            try
            {
                // VERIFICAR SI EL REGISTRO EXISTE
                Cliente cliente = await _dbContext.Clientes.FindAsync(id);
                if (cliente == null)
                {
                    return false;
                }
                // SI EXISTE BORRAMOS EL CLIENTE
                _dbContext.Clientes.Remove(cliente);
                // GRABAMOS TODOS LOS CAMBIOS
                await _dbContext.SaveChangesAsync();

                return true;

            }
            catch (System.Exception)
            {

                return false;
            }
        }

        public async Task<ClienteDto> GetClienteById(int id)
        {
            // OBTENEMOS EL CLIENTE POR EL ID
            Cliente cliente = await _dbContext.Clientes.FindAsync(id);

            // HACERMOS UN MAPEO PARA RETORNAR EL CLIENTEDTO pasando el OBJ cliente
            return _mapper.Map<ClienteDto>(cliente);

        }

        public async Task<List<ClienteDto>> GetClientes()
        {
            // OBTENEMOS LA LISTA DE CLIENTES
            List<Cliente> listaClientes = await _dbContext.Clientes.ToListAsync();

            // HACEMOS EL MAPEO PARA RETORNAR DE TIPO CLIENTEDTO pasando la VARIABLE listaCliente
            return _mapper.Map<List<ClienteDto>>(listaClientes);
        }
    }
}
