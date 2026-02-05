export abstract class ApiRoutes {
  private static readonly BASE_URL = "http://localhost:5173/api";

  public static readonly Usuario = {
    Registrar: `${this.BASE_URL}/Usuario`,
    Login: `${this.BASE_URL}/Usuario/login`,
    ObtenerPorId: (id: number) => `${this.BASE_URL}/Usuario/${id}`,
    CambiarPassword: `${this.BASE_URL}/Usuario/me/password`,
    Refresh: `${this.BASE_URL}/Usuario/refresh`,
    Logout: `${this.BASE_URL}/Usuario/logout`,
  };

  public static readonly Paciente = {
    Registrar: `${this.BASE_URL}/Paciente`,
    ObtenerMiPerfil: `${this.BASE_URL}/Paciente/me`, //GET
    ModificarMiPerfil: `${this.BASE_URL}/Paciente/me`, //PUT
    ActualizarPaciente: (id: number) => `${this.BASE_URL}/Paciente/${id}`,
    Listar: (estado?: number, page: number = 1, size: number = 10) => {
      let url = `${this.BASE_URL}/Paciente?page=${page}&size=${size}`;
      if (estado !== undefined) {
        url += `&estado=${estado}`;
      }
      return url;
    },
    CambiarEstado: (id: number) => `${this.BASE_URL}/Paciente/${id}/estado`,
  };

  public static readonly Nutricionista = {
    Registrar: `${this.BASE_URL}/Nutricionista`,
    ObtenerMiPerfil: `${this.BASE_URL}/Nutricionista/me`, //GET
    ModificarMiPerfil: `${this.BASE_URL}/Nutricionista/me`, //PUT
  };

  public static readonly Pesaje = {
    Registrar: `${this.BASE_URL}/Pesaje`,
    ObtenerPorId: (id: number) => `${this.BASE_URL}/Pesaje/${id}`,
    ObtenerMiHistorial: (page: number = 1, size: number = 10) =>
      `${this.BASE_URL}/Pesaje/me?page=${page}&size=${size}`,
    Eliminar: (id: number) => `${this.BASE_URL}/Pesaje/${id}`,
  };

  public static readonly Dieta = {
    Registrar: `${this.BASE_URL}/Dieta`,
    ObtenerPorId: (id: number) => `${this.BASE_URL}/Dieta/${id}`, //GET
    Modificar: (id: number) => `${this.BASE_URL}/Dieta/${id}`, //PUT
    Eliminar: (id: number) => `${this.BASE_URL}/Dieta/${id}`, //DELETE
    ObtenerHistorial: (id: number) =>
      `${this.BASE_URL}/Dieta/Paciente/${id}/historial`,
    ObtenerDietaActiva: (id: number) =>
      `${this.BASE_URL}/Dieta/Paciente/${id}/activa`,
    ActivarDieta: (idDieta: number, idPaciente: Number) =>
      `${this.BASE_URL}/${idDieta}/activar/${idPaciente}`,
  };

  public static readonly Comida = {
    Crear: `${this.BASE_URL}/Comida`,
    Listar: (
      idCategoria?: number,
      nombre?: string,
      page: number = 1,
      size: number = 10,
    ) => {
      let url = `${this.BASE_URL}/Comida?page=${page}&size=${size}`;

      if (idCategoria !== undefined && idCategoria !== null) {
        url += `&idCategoria=${idCategoria}`;
      }

      if (nombre) {
        url += `&nombre=${encodeURIComponent(nombre)}`;
      }

      return url;
    },
    ObtenerPorId: (id: number) => `${this.BASE_URL}/Comida/${id}`, //GET
    Eliminar: (id: number) => `${this.BASE_URL}/Comida/${id}`, //DELETE
    Modificar: (id: number) => `${this.BASE_URL}/Comida/${id}`, //PUT
  };

  public static readonly Categoria = {
    Crear: `${this.BASE_URL}/Categoria`,
    ObtenerPorId: (id: number) => `${this.BASE_URL}/Categoria/${id}`, //GET
    Modificar: (id: number) => `${this.BASE_URL}/Categoria/${id}`, //PUT
    ObtenerTodas: `${this.BASE_URL}/Categoria`, //GET
    Eliminar: (id: number) => `${this.BASE_URL}/Categoria/${id}`, //DELETE
  };
}
