export interface ComidaRequestDTO{
    Nombre: string;
    Descripcion?: string;
    Proteinas: number;
    Carbohidratos: number;
    Grasas: number;
    Fibra?: number;
    Azucares?: number;
    Porcion: number;
    Imagen_Url?: string;
    ids_Categorias: number[];
}