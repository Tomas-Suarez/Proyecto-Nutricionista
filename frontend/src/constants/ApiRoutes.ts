export abstract class ApiRoutes {
  public static readonly BASE_URL = import.meta.env.VITE_API_BASE_URL;

  public static readonly Usuario = {
    Registrar: `/Usuario`,
    Login: `/Usuario/login`,
    ObtenerPorId: (id: number) => `/Usuario/${id}`,
    CambiarPassword: `/Usuario/me/password`,
    Refresh: `/Usuario/refresh`,
    Logout: `/Usuario/logout`,
    SubirAvatar: `/Usuario/avatar`, //POST
    BorrarAvatar: `/Usuario/avatar`, //DELETE
  };

  public static readonly Paciente = {
    Registrar: `/Paciente`,
    AccederPublico: `/Paciente/acceder`,
    ActualizarPaciente: (id: number) => `/Paciente/${id}`,
    Listar: (estado?: number, busqueda?: string, page: number = 1, size: number = 10) => {
      let url = `/Paciente?page=${page}&size=${size}`;
      if (estado !== undefined && estado !== null) url += `&estado=${estado}`;
      if (busqueda) url += `&busqueda=${encodeURIComponent(busqueda)}`;
      return url;
    },
    CambiarEstado: (id: number) => `/Paciente/${id}/estado`,
    ObtenerPorId: (id: number) => `/paciente/${id}`,
  };

  public static readonly Nutricionista = {
    Registrar: `/Nutricionista`,
    ObtenerMiPerfil: `/Nutricionista/me`, //GET
    ModificarMiPerfil: `/Nutricionista/me`, //PUT
  };

  public static readonly Pesaje = {
    Registrar: `/Pesaje`,
    ObtenerPorId: (id: number) => `/Pesaje/${id}`,
    ObtenerHistorialPorPaciente: (idPaciente: number, page: number = 1, size: number = 10) =>
      `/Pesaje/historial/${idPaciente}?page=${page}&size=${size}`,
    ObtenerHistorialPublico: (page: number = 1, size: number = 10) =>
      `/Pesaje/publico/historial?page=${page}&size=${size}`,
    Eliminar: (id: number) => `/Pesaje/${id}`,
  };

  public static readonly Dieta = {
    Registrar: `/Dieta`,
    ObtenerPorId: (id: number) => `/Dieta/${id}`,
    Modificar: (id: number) => `/Dieta/${id}`,
    Eliminar: (id: number) => `/Dieta/${id}`,
    ObtenerHistorial: (id: number, page: number = 1, size: number = 10) => 
      `/Dieta/paciente/${id}/historial?page=${page}&size=${size}`,
    ObtenerDietaActiva: (id: number) => `/Dieta/Paciente/${id}/activa`,
    ActivarDieta: (idDieta: number) => `/Dieta/${idDieta}/activar`,
    ObtenerDietaPublica: `/Dieta/publico/activa`,
  };

  public static readonly Comida = {
    Crear: `/Comida`,
    Listar: (
      idCategoria?: number,
      nombre?: string,
      page: number = 1,
      size: number = 10,
    ) => {
      let url = `/Comida?page=${page}&size=${size}`;

      if (idCategoria !== undefined && idCategoria !== null) {
        url += `&idCategoria=${idCategoria}`;
      }

      if (nombre) {
        url += `&nombre=${encodeURIComponent(nombre)}`;
      }

      return url;
    },
    ObtenerPorId: (id: number) => `/Comida/${id}`, //GET
    Eliminar: (id: number) => `/Comida/${id}`, //DELETE
    Modificar: (id: number) => `/Comida/${id}`, //PUT
  };

  public static readonly Categoria = {
    Crear: `/Categoria`,
    ObtenerPorId: (id: number) => `/Categoria/${id}`, //GET
    Modificar: (id: number) => `/Categoria/${id}`, //PUT
    ObtenerTodas: `/Categoria`, //GET
    Eliminar: (id: number) => `/Categoria/${id}`, //DELETE
  };

  public static readonly Patologia = {
    Crear: `/Patologia`,
    ObtenerPorId: (id: number) => `/Patologia/${id}`, //GET
    Modificar: (id: number) => `/Patologia/${id}`, //PUT
    ObtenerTodas: `/Patologia`, //GET
    Eliminar: (id: number) => `/Patologia/${id}`, //DELETE
  };

  public static readonly Objetivo = {
    Crear: `/Objetivo`,
    ObtenerPorId: (id: number) => `/Objetivo/${id}`, //GET
    Modificar: (id: number) => `/Objetivo/${id}`, //PUT
    ObtenerTodas: `/Objetivo`, //GET
    Eliminar: (id: number) => `/Objetivo/${id}`, //DELETE
  };
}
