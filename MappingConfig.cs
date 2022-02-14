using APIClientes.Models;
using APIClientes.Models.Dto;
using AutoMapper;

namespace APIClientes
{
    public class MappingConfig
    {
        // AQUI ES DONDE SE VA HACER EL MAPEO ENTRE LOS DTO Y NUESTROS MODELOS
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                // NUESTRO CLIENTEDTO VA A ESTAR MAPEADO CON NUESTRO MODELO CLIENTE
                config.CreateMap<ClienteDto, Cliente>();

                // HACEMOS INGENERIA INVERSA PARA MAPEAR NUESTRO CLIENTE CON DTO
                config.CreateMap<Cliente, ClienteDto>();
            });

            return mappingConfig;

        }
    }
}
