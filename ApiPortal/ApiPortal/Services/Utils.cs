namespace ApiPortal.Services
{
    public class Utils
    {
        public string nombreArchivo(string nombre, int numeroImagen)
        {
            string[] archivo = nombre.Split('.');

            switch (numeroImagen)
            {
                case 1: //LogoPortada
                    return "LogoPortada." + archivo[1];                    
                    break;
                case 2: //ImagenPortada
                    return "ImagenPortada." + archivo[1];
                    break;
                case 3: //LogoSidebar
                    return "LogoSidebar." + archivo[1];
                    break;
                case 4: //LogoMinimalistaSidebar
                    return "LogoMinimalistaSidebar." + archivo[1];
                    break;
                case 5: //BannerPagoRapido
                    return "BannerPagoRapido." + archivo[1];
                    break;
                case 6: //ImagenUltimasCompras
                    return "ImagenUltimasCompras." + archivo[1];
                    break;
                case 7: //IconoMisCompras
                    return "IconoMisCompras." + archivo[1];
                    break;
                case 8: //BannerMisCompras
                    return "BannerMisCompras." + archivo[1];
                    break;
                case 9: //ImagenUsuario
                    return "ImagenUsuario." + archivo[1];
                    break;
                case 10: //BannerPortal
                    return "BannerPortal." + archivo[1];
                    break;
                case 11: //IconoContactos
                    return "IconoContactos." + archivo[1];
                    break;
                case 12: //IconoClavePerfil
                    return "IconoClavePerfil." + archivo[1];
                    break;
                case 13: //IconoEditarPerfil
                    return "IconoEditarPerfil." + archivo[1];
                    break;
                case 14: //IconoEstadoPerfil
                    return "IconoEstadoPerfil." + archivo[1];
                    break;
                case 15: //IconoEstadoPerfil
                    return "LogoCorreo." + archivo[1];
                    break;
            }

            return string.Empty;
        }
    }
}
