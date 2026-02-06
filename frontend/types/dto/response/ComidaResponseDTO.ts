export interface ComidaResponseDTO{
    Id_Comida: number;
    Nombre: string;
    Descripcion?: string;
    Calorias: number;
    Proteinas: number;
    Carbohidratos: number;
    Grasas: number;
    Fibra: number;
    Azucares: number;
    Porcion: string;
    Imagen_Url: string;
    Categorias: string[];
}