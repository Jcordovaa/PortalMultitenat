CREATE TABLE [dbo].[Acceso](
	[IdAcceso] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](100) NULL,
	[MenuPadre] [int] NULL,
	[Activo] [int] NULL,
 CONSTRAINT [PK_Acceso] PRIMARY KEY CLUSTERED 
(
	[IdAcceso] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApiSoftland]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApiSoftland](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Ambiente] [varchar](50) NULL,
	[Url] [varchar](500) NULL,
	[Token] [varchar](500) NULL,
	[AreaDatos] [varchar](100) NULL,
	[ConsultaTiposDeDocumentos] [varchar](500) NULL,
	[ConsultaPlanDeCuentas] [varchar](500) NULL,
	[ConsultaRegiones] [varchar](500) NULL,
	[ConsultaComunas] [varchar](500) NULL,
	[ConsultaGiros] [varchar](500) NULL,
	[ContactosXAuxiliar] [varchar](500) NULL,
	[ConsultaCliente] [varchar](500) NULL,
	[ActualizaCliente] [varchar](500) NULL,
	[ResumenContable] [varchar](500) NULL,
	[CapturaComprobantes] [varchar](500) NULL,
	[DocumentosFacturados] [varchar](500) NULL,
	[DetalleDocumento] [varchar](500) NULL,
	[ObtenerPdfDocumento] [varchar](500) NULL,
	[DespachoDeDocumento] [varchar](500) NULL,
	[ProductosComprados] [varchar](500) NULL,
	[PendientesPorFacturar] [varchar](500) NULL,
	[DetalleNotaDeVenta] [varchar](500) NULL,
	[ObtenerPdf] [varchar](500) NULL,
	[ObtieneGuiasPendientes] [varchar](500) NULL,
	[login] [varchar](500) NULL,
	[ObtieneVendedores] [varchar](500) NULL,
	[ObtieneCondicionesVenta] [varchar](500) NULL,
	[ObtieneListasPrecio] [varchar](500) NULL,
	[ObtieneCategoriaClientes] [varchar](500) NULL,
	[DocumentosContabilizados] [varchar](1000) NULL,
	[ObtieneModulos] [varchar](500) NULL,
	[ConsultaMonedas] [varchar](500) NULL,
	[ContabilizaPagos] [varchar](500) NULL,
	[ConsultaCargos] [varchar](500) NULL,
	[DocumentosContabilizadosResumen] [varchar](500) NULL,
	[TopDeudores] [varchar](500) NULL,
	[TransbankRegistrarCliente] [varchar](500) NULL,
	[DocContabilizadosResumenxRut] [varchar](1000) NULL,
	[PagosxDocumento] [varchar](500) NULL,
	[HabilitaLogApi] [int] NULL,
	[CadenaAlmacenamientoAzure] [varchar](500) NULL,
	[UrlAlmacenamientoArchivos] [varchar](500) NULL,
	[CuentasPasarelaPagos] [varchar](500) NULL,
	[ConsultaAuxiliar] [varchar](500) NULL,
	[ReintentosCallback] [int] NULL,
	[ReintentosRedirect] [int] NULL,
	[MilisegundosReintoCalback] [int] NULL,
	[MiliSegundosReintentoRedirect] [int] NULL,
 CONSTRAINT [PK_ApiSoftland] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Automatizacion]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Automatizacion](
	[IdAutomatizacion] [int] IDENTITY(1,1) NOT NULL,
	[IdTipoAutomatizacion] [int] NOT NULL,
	[Anio] [int] NULL,
	[TipoDocumentos] [varchar](200) NULL,
	[DiasVencimiento] [int] NULL,
	[IdHorario] [int] NULL,
	[IdPerioricidad] [int] NULL,
	[ExcluyeFestivos] [int] NULL,
	[ExcluyeClientes] [int] NULL,
	[CodCategoriaCliente] [varchar](max) NULL,
	[CodListaPrecios] [varchar](max) NULL,
	[CodCondicionVenta] [varchar](max) NULL,
	[CodVendedor] [varchar](max) NULL,
	[MuestraSoloVencidos] [int] NULL,
	[AgrupaCobranza] [int] NULL,
	[Estado] [int] NULL,
	[DiasRecordatorio] [int] NULL,
	[DiaEnvio] [int] NULL,
	[CodCanalVenta] [varchar](max) NULL,
	[CodCobrador] [varchar](max) NULL,
	[CodCargo] [varchar](max) NULL,
	[EnviaTodosContactos] [int] NULL,
	[EnviaCorreoFicha] [int] NULL,
	[Nombre] [varchar](1000) NULL,
 CONSTRAINT [PK_Automatizacion] PRIMARY KEY CLUSTERED 
(
	[IdAutomatizacion] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClientesExcluidos]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientesExcluidos](
	[IdExcluido] [int] IDENTITY(1,1) NOT NULL,
	[RutCliente] [varchar](50) NULL,
	[CodAuxCliente] [varchar](50) NULL,
	[NombreCliente] [varchar](60) NULL,
 CONSTRAINT [PK_AlumnosExcluidos] PRIMARY KEY CLUSTERED 
(
	[IdExcluido] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClientesPortal]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientesPortal](
	[IdCliente] [int] IDENTITY(1,1) NOT NULL,
	[Rut] [varchar](50) NULL,
	[CodAux] [varchar](50) NULL,
	[Nombre] [varchar](100) NULL,
	[Correo] [varchar](100) NULL,
	[Clave] [varchar](100) NULL,
	[ActivaCuenta] [int] NULL,
 CONSTRAINT [PK_ClientesPortal] PRIMARY KEY CLUSTERED 
(
	[IdCliente] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CobranzaCabecera]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CobranzaCabecera](
	[IdCobranza] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](60) NULL,
	[FechaCreacion] [date] NULL,
	[HoraCreacion] [varchar](10) NULL,
	[IdTipoCobranza] [int] NULL,
	[Estado] [int] NULL,
	[IdUsuario] [int] NULL,
	[TipoProgramacion] [int] NULL,
	[FechaInicio] [date] NULL,
	[FechaFin] [date] NULL,
	[HoraDeEnvio] [int] NULL,
	[DiaSemanaEnvio] [varchar](50) NULL,
	[DiasToleranciaVencimiento] [int] NULL,
	[IdEstado] [int] NULL,
	[Anio] [int] NULL,
	[TipoDocumento] [varchar](max) NULL,
	[FechaDesde] [date] NULL,
	[FechaHasta] [date] NULL,
	[AplicaClientesExcluidos] [int] NULL,
	[EsCabeceraInteligente] [int] NULL,
	[IdCabecera] [int] NULL,
	[EnviaEnlacePago] [int] NULL,
	[IdPeriodicidad] [int] NULL,
	[ExcluyeFeriado] [int] NULL,
	[ExcluyeFinDeSemana] [int] NULL,
	[DiaEnvio] [int] NULL,
	[DesdeMontoDeuda] [int] NULL,
	[HastaMontoDeuda] [int] NULL,
	[CantidadDocumentos] [int] NULL,
	[EjecutaSiguienteHabil] [int] NULL,
	[ListaPrecio] [varchar](max) NULL,
	[CategoriaCliente] [varchar](max) NULL,
	[CondicionVenta] [varchar](max) NULL,
	[Vendedor] [varchar](max) NULL,
	[CargosContactos] [varchar](max) NULL,
	[EnviaTodosContactos] [int] NULL,
	[EnviaCorreoFicha] [int] NULL,
	[EnviaTodosCargos] [int] NULL,
	[CanalesVenta] [varchar](max) NULL,
	[Cobradores] [varchar](max) NULL,
 CONSTRAINT [PK_CobranzaCabecera] PRIMARY KEY CLUSTERED 
(
	[IdCobranza] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CobranzaDetalle]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CobranzaDetalle](
	[IdCobranzaDetalle] [int] IDENTITY(1,1) NOT NULL,
	[IdCobranza] [int] NULL,
	[Folio] [int] NULL,
	[FechaEmision] [date] NULL,
	[FechaVencimiento] [date] NULL,
	[Monto] [real] NULL,
	[RutCliente] [varchar](50) NULL,
	[CodAuxCliente] [varchar](50) NULL,
	[TipoDocumento] [varchar](50) NULL,
	[IdEstado] [int] NULL,
	[FechaEnvio] [date] NULL,
	[HoraEnvio] [varchar](50) NULL,
	[FechaPago] [date] NULL,
	[HoraPago] [varchar](50) NULL,
	[ComprobanteContable] [varchar](50) NULL,
	[FolioDTE] [varchar](50) NULL,
	[IdPago] [int] NULL,
	[CuentaContable] [varchar](50) NULL,
	[EmailCliente] [varchar](150) NULL,
	[NombreCliente] [varchar](60) NULL,
	[Pagado] [real] NULL,
 CONSTRAINT [PK_CobranzaDetalle] PRIMARY KEY CLUSTERED 
(
	[IdCobranzaDetalle] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CobranzaHorarios]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CobranzaHorarios](
	[IdHorario] [int] IDENTITY(1,1) NOT NULL,
	[Horario] [varchar](50) NULL,
	[Hora] [int] NULL,
	[Minuto] [int] NULL,
 CONSTRAINT [PK_CobranzaHorarios] PRIMARY KEY CLUSTERED 
(
	[IdHorario] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CobranzaPeriocidad]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CobranzaPeriocidad](
	[IdPeriocidad] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
	[DiaMes] [int] NULL,
	[Estado] [int] NULL,
 CONSTRAINT [PK_CobranzaPeriocidad] PRIMARY KEY CLUSTERED 
(
	[IdPeriocidad] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfigracionCobranzas]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfigracionCobranzas](
	[IdConfigCobranza] [int] IDENTITY(1,1) NOT NULL,
	[TipoDocumentoCobranza] [varchar](max) NULL,
	[CondicionesCredito] [varchar](max) NULL,
	[EnviaCobranza] [int] NULL,
	[CantidadDiasVencimiento] [int] NULL,
	[IdFrecuenciaEnvioCob] [int] NULL,
	[EnviaPreCobranza] [int] NULL,
	[CantidadDiasPrevios] [int] NULL,
	[IdFrecuenciaEnvioPre] [int] NULL,
 CONSTRAINT [PK_ConfigracionCobranzas] PRIMARY KEY CLUSTERED 
(
	[IdConfigCobranza] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfiguracionCorreo]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfiguracionCorreo](
	[IdConfiguracionCorreo] [int] IDENTITY(1,1) NOT NULL,
	[SmtpServer] [varchar](100) NULL,
	[Usuario] [varchar](100) NULL,
	[Clave] [varchar](255) NULL,
	[Puerto] [int] NULL,
	[SSL] [int] NULL,
	[CorreoAvisoPago] [varchar](1000) NULL,
	[AsuntoPagoCliente] [varchar](170) NULL,
	[AsuntoAccesoCliente] [varchar](170) NULL,
	[NombreCorreos] [varchar](50) NULL,
	[AsuntoEnvioDocumentos] [varchar](170) NULL,
	[TextoEnvioDocumentos] [varchar](500) NULL,
	[CantidadCorreosAcceso] [int] NULL,
	[CantidadCorreosNotificacion] [int] NULL,
	[LogoCorreo] [varchar](500) NULL,
	[TextoMensajeActivacion] [varchar](500) NULL,
	[TituloPagoCliente] [varchar](170) NULL,
	[TituloAccesoCliente] [varchar](170) NULL,
	[TituloEnvioDocumentos] [varchar](170) NULL,
	[TituloCambioDatos] [varchar](170) NULL,
	[TituloCambioClave] [varchar](170) NULL,
	[TituloRecuperarClave] [varchar](170) NULL,
	[AsuntoCambioDatos] [varchar](170) NULL,
	[AsuntoCambioClave] [varchar](170) NULL,
	[AsuntoRecuperarClave] [varchar](170) NULL,
	[TextoPagoCliente] [varchar](500) NULL,
	[TextoAccesoCliente] [varchar](500) NULL,
	[TextoCambioDatos] [varchar](500) NULL,
	[TextoCambioClave] [varchar](500) NULL,
	[TextoRecuperarClave] [varchar](500) NULL,
	[TituloAvisoPagoCliente] [varchar](1700) NULL,
	[AsuntoAvisoPagoCliente] [varchar](170) NULL,
	[TextoAvisoPagoCliente] [varchar](500) NULL,
	[ColorBoton] [varchar](50) NULL,
	[TituloPagoSinComprobante] [varchar](170) NULL,
	[AsuntoPagoSinComprobante] [varchar](170) NULL,
	[TextoPagoSinComprobante] [varchar](500) NULL,
	[AsuntoCambioCorreo] [varchar](170) NULL,
	[TituloCambioCorreo] [varchar](170) NULL,
	[TextoCambioCorreo] [varchar](500) NULL,
	[TituloCobranza] [varchar](170) NULL,
	[TextoCobranza] [varchar](500) NULL,
	[AsuntoCobranza] [varchar](170) NULL,
	[TituloPreCobranza] [varchar](170) NULL,
	[AsuntoPreCobranza] [varchar](170) NULL,
	[TextoPreCobranza] [varchar](500) NULL,
	[TextoEstadoCuenta] [varchar](500) NULL,
	[TituloEstadoCuenta] [varchar](170) NULL,
	[AsuntoEstadoCuenta] [varchar](170) NULL,
	[CorreoOrigen] [varchar](100) NULL,
	[UtilizaSES] [int] NULL,
 CONSTRAINT [PK_ConfiguracionCorreo] PRIMARY KEY CLUSTERED 
(
	[IdConfiguracionCorreo] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfiguracionCorreoCasillas]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfiguracionCorreoCasillas](
	[IdCasilla] [int] IDENTITY(1,1) NOT NULL,
	[Casilla] [varchar](100) NULL,
	[CantidadDia] [int] NULL,
 CONSTRAINT [PK_ConfiguracionCorreoCasillas] PRIMARY KEY CLUSTERED 
(
	[IdCasilla] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfiguracionDiseno]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfiguracionDiseno](
	[IdConfiguracionTextosColores] [int] IDENTITY(1,1) NOT NULL,
	[TituloResumenContable] [varchar](60) NULL,
	[TituloUltimasCompras] [varchar](60) NULL,
	[TituloMisCompras] [varchar](60) NULL,
	[TituloComprasFacturadas] [varchar](60) NULL,
	[TituloPendientesFacturar] [varchar](60) NULL,
	[TituloProductos] [varchar](60) NULL,
	[TituloGuiasPendientes] [varchar](60) NULL,
	[ImagenUltimasCompras] [varchar](500) NULL,
	[BannerPortal] [varchar](500) NULL,
	[ImagenUsuario] [varchar](500) NULL,
	[IconoContactos] [varchar](500) NULL,
	[BannerMisCompras] [varchar](500) NULL,
	[IconoMisCompras] [varchar](500) NULL,
	[ColorTextoUltimasCompras] [varchar](50) NULL,
	[ColorBotonUltimasCompras] [varchar](50) NULL,
	[ColorHoverBotonUltimasCompras] [varchar](50) NULL,
	[ColorIconoPendientes] [varchar](50) NULL,
	[ColorTextoPendientes] [varchar](50) NULL,
	[ColorIconoVencidos] [varchar](50) NULL,
	[ColorTextoVencidos] [varchar](50) NULL,
	[ColorIconoPorVencer] [varchar](50) NULL,
	[ColorTextoPorVencer] [varchar](50) NULL,
	[ColorSeleccionDocumentos] [varchar](50) NULL,
	[IconoClavePerfil] [varchar](500) NULL,
	[IconoEditarPerfil] [varchar](500) NULL,
	[IconoEstadoPerfil] [varchar](500) NULL,
	[ColorBotonModificarPerfil] [varchar](50) NULL,
	[ColorBotonClavePerfil] [varchar](50) NULL,
	[ColorBotonEstadoPerfil] [varchar](50) NULL,
	[ColorHoverBotonesPerfil] [varchar](50) NULL,
	[ColorBotonCancelarModalPerfil] [varchar](50) NULL,
	[ColorBotonGuardarModalPerfil] [varchar](50) NULL,
	[ColorFondoMisCompras] [varchar](50) NULL,
	[ColorIconosMisCompras] [varchar](50) NULL,
	[ColorTextoMisCompras] [varchar](50) NULL,
	[ColorBotonBuscar] [varchar](50) NULL,
	[ColorPaginador] [varchar](50) NULL,
	[ColorTextoFechaUltimasCompras] [varchar](50) NULL,
	[ColorTextoMontoUltimasCompras] [varchar](50) NULL,
	[ColorTextoBotonUltimasCompras] [varchar](50) NULL,
	[ColorFondoDocumentos] [varchar](50) NULL,
	[ColorFondoUltimasCompras] [varchar](50) NULL,
	[ColorFondoResumenContable] [varchar](50) NULL,
	[ColorFondoVencidos] [varchar](50) NULL,
	[ColorFondoPorVencer] [varchar](50) NULL,
	[TituloPendientesDashboard] [varchar](70) NULL,
	[TituloVencidosDashboard] [varchar](70) NULL,
	[TituloPorVencerDashboard] [varchar](70) NULL,
	[TituloMonedaPeso] [varchar](70) NULL,
	[TituloOtraMoneda] [varchar](70) NULL,
	[ColorFondoPendientesMisCompras] [varchar](50) NULL,
	[ColorFondoProductosMisCompras] [varchar](50) NULL,
	[ColorFondoGuiasMisCompras] [varchar](50) NULL,
	[TextoNoConsideraTodaDeuda] [varchar](max) NULL,
	[ImagenPortada] [varchar](500) NULL,
	[ColorFondoPortada] [varchar](50) NULL,
	[LogoPortada] [varchar](500) NULL,
	[ColorBotonInicioSesion] [varchar](50) NULL,
	[ColorBotonPagoRapido] [varchar](50) NULL,
	[ColorBotonPagar] [varchar](50) NULL,
	[BannerPagoRapido] [varchar](500) NULL,
	[LogoMinimalistaSidebar] [varchar](500) NULL,
	[LogoSidebar] [varchar](500) NULL,
	[Favicon] [varchar](500) NULL,
	[TextoCobranzaExpirada] [varchar](max) NULL,
	[TextoDescargaCobranza] [varchar](max) NULL,
 CONSTRAINT [PK_ConfiguracionDiseno] PRIMARY KEY CLUSTERED 
(
	[IdConfiguracionTextosColores] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfiguracionEmpresa]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfiguracionEmpresa](
	[IdConfiguracionEmpresa] [int] IDENTITY(1,1) NOT NULL,
	[RutEmpresa] [varchar](50) NULL,
	[NombreEmpresa] [varchar](250) NULL,
	[Direccion] [varchar](250) NULL,
	[RutaGoogleMaps] [varchar](500) NULL,
	[Telefono] [varchar](20) NULL,
	[CorreoContacto] [varchar](100) NULL,
	[Facebook] [varchar](200) NULL,
	[Instagram] [varchar](200) NULL,
	[Twitter] [varchar](200) NULL,
	[Youtube] [varchar](200) NULL,
	[Linkedin] [varchar](200) NULL,
	[UrlPortal] [varchar](1000) NULL,
	[Logo] [varchar](1000) NULL,
	[Web] [varchar](1000) NULL,
 CONSTRAINT [PK_ConfiguracionEmpresa] PRIMARY KEY CLUSTERED 
(
	[IdConfiguracionEmpresa] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfiguracionPagoCliente]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfiguracionPagoCliente](
	[IdConfiguracionPago] [int] IDENTITY(1,1) NOT NULL,
	[CuentasContablesDeuda] [varchar](max) NULL,
	[TiposDocumentosDeuda] [varchar](max) NULL,
	[AnioTributario] [int] NULL,
	[MonedaUtilizada] [varchar](50) NULL,
	[GlosaComprobante] [varchar](60) NULL,
	[CentroCosto] [varchar](50) NULL,
	[AreaNegocio] [varchar](50) NULL,
	[DiasPorVencer] [int] NULL,
	[DocumentosCobranza] [varchar](max) NULL,
	[GlosaDetalle] [varchar](60) NULL,
	[GlosaPago] [varchar](60) NULL,
	[SegundaMonedaUtilizada] [varchar](50) NULL,
 CONSTRAINT [PK_ConfiguracionPago] PRIMARY KEY CLUSTERED 
(
	[IdConfiguracionPago] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfiguracionPortal]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfiguracionPortal](
	[IdConfiguracionPortal] [int] IDENTITY(1,1) NOT NULL,
	[CrmSoftland] [int] NULL,
	[InventarioSoftland] [int] NULL,
	[ContabilidadSoftland] [int] NULL,
	[NotaVentaSoftland] [int] NULL,
	[CantidadUltimasCompras] [int] NULL,
	[MuestraEstadoBloqueo] [int] NULL,
	[MuestraEstadoSobregiro] [int] NULL,
	[MuestraContactoComercial] [int] NULL,
	[MuestraContactosPerfil] [int] NULL,
	[MuestraContactosEnvio] [int] NULL,
	[HabilitaPagoRapido] [int] NULL,
	[ImagenCaberaPerfil] [varchar](500) NULL,
	[ImagenCabeceraCompras] [varchar](500) NULL,
	[PermiteExportarExcel] [int] NULL,
	[PermiteAbonoParcial] [int] NULL,
	[UtilizaNumeroDireccion] [int] NULL,
	[CantUltPagosRec] [int] NULL,
	[CantUltPagosAnul] [int] NULL,
	[MuestraUltimasCompras] [int] NULL,
	[MuestraBotonImprimir] [int] NULL,
	[MuestraBotonEnviarCorreo] [int] NULL,
	[MuestraResumen] [int] NULL,
	[ResumenContabilizado] [int] NULL,
	[MuestraComprasFacturadas] [int] NULL,
	[MuestraPendientesFacturar] [int] NULL,
	[MuestraProductos] [int] NULL,
	[MuestraGuiasPendientes] [int] NULL,
	[UtilizaDocumentoPagoRapido] [int] NULL,
	    [EstadoImplementacion] [int] NULL, 
 CONSTRAINT [PK_ConfiguracionPortal] PRIMARY KEY CLUSTERED 
(
	[IdConfiguracionPortal] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfiguracionTiposDocumentos]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfiguracionTiposDocumentos](
	[IdTipoDocConfig] [int] IDENTITY(1,1) NOT NULL,
	[IdConfiguracion] [int] NULL,
	[Nombre] [varchar](60) NULL,
	[CodErp] [varchar](60) NULL,
 CONSTRAINT [PK_ConfiguracionTiposDocumentos] PRIMARY KEY CLUSTERED 
(
	[IdTipoDocConfig] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CorreoDTE]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CorreoDTE](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Rut] [varchar](50) NULL,
	[Correo] [varchar](200) NULL,
 CONSTRAINT [PK_CorreoDTE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EstadoCobranza]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EstadoCobranza](
	[IdEstadoCobranza] [int] NOT NULL,
	[Nombre] [varchar](50) NULL,
 CONSTRAINT [PK_EstadoCobranza] PRIMARY KEY CLUSTERED 
(
	[IdEstadoCobranza] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feriados]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feriados](
	[IdFeriado] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
	[Fecha] [date] NULL,
	[Anio] [int] NULL,
 CONSTRAINT [PK_Feriados] PRIMARY KEY CLUSTERED 
(
	[IdFeriado] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Log_Softlandpay]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log_Softlandpay](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Origen] [varchar](500) NULL,
	[IdTransaccion] [varchar](500) NULL,
	[MedioPago] [varchar](50) NULL,
	[Estado] [varchar](500) NULL,
	[MontoBruto] [varchar](50) NULL,
	[MontoTotal] [varchar](50) NULL,
	[MontoImpuestos] [varchar](50) NULL,
	[Fecha] [varchar](50) NULL,
	[IdInterno] [varchar](50) NULL,
	[Request] [varchar](max) NULL,
 CONSTRAINT [PK_Log_Softlandpay] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogApi]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogApi](
	[Id] [varchar](100) NOT NULL,
	[Api] [varchar](max) NULL,
	[Inicio] [datetime] NULL,
	[Termino] [datetime] NULL,
	[Segundos] [int] NULL,
 CONSTRAINT [PK_LogApi_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogApiDetalle]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogApiDetalle](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdLogApi] [varchar](100) NULL,
	[Metodo] [varchar](max) NULL,
	[Inicio] [datetime] NULL,
	[Termino] [datetime] NULL,
	[Segundos] [int] NULL,
 CONSTRAINT [PK_LogApiDetalle] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogCobranzas]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogCobranzas](
	[IdLogCobranza] [int] IDENTITY(1,1) NOT NULL,
	[FechaInicio] [datetime] NULL,
	[FechaTermino] [datetime] NULL,
	[Estado] [varchar](50) NULL,
	[CobranzasConsideradas] [varchar](1000) NULL,
	[AnioProceso] [int] NULL,
 CONSTRAINT [PK_LogCobranzas] PRIMARY KEY CLUSTERED 
(
	[IdLogCobranza] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogCorreos]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogCorreos](
	[IdLogEmail] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [datetime] NULL,
	[Rut] [varchar](50) NULL,
	[CodAux] [varchar](50) NULL,
	[Tipo] [varchar](50) NULL,
	[Estado] [varchar](50) NULL,
	[Error] [varchar](max) NULL,
	[TipoCorreo] [int] NULL,
 CONSTRAINT [PK_LogCorreos] PRIMARY KEY CLUSTERED 
(
	[IdLogEmail] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogProcesos]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogProcesos](
	[IdTipoProceso] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [date] NULL,
	[Hora] [varchar](50) NULL,
	[Ruta] [varchar](100) NULL,
	[Mensaje] [varchar](max) NULL,
	[Excepcion] [varchar](max) NULL,
 CONSTRAINT [PK_LogProcesos] PRIMARY KEY CLUSTERED 
(
	[IdTipoProceso] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PagosCabecera]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PagosCabecera](
	[IdPago] [int] IDENTITY(1,1) NOT NULL,
	[IdCliente] [int] NULL,
	[FechaPago] [date] NULL,
	[HoraPago] [varchar](10) NULL,
	[MontoPago] [real] NULL,
	[ComprobanteContable] [varchar](100) NULL,
	[IdPagoEstado] [int] NULL,
	[Rut] [varchar](50) NULL,
	[CodAux] [varchar](50) NULL,
	[Nombre] [varchar](60) NULL,
	[Correo] [varchar](50) NULL,
	[IdPasarela] [int] NULL,
	[EsPagoRapido] [int] NULL,
 CONSTRAINT [PK_PagosCabecera] PRIMARY KEY CLUSTERED 
(
	[IdPago] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PagosDetalle]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PagosDetalle](
	[IdPagoDetalle] [int] IDENTITY(1,1) NOT NULL,
	[IdPago] [int] NOT NULL,
	[Folio] [int] NULL,
	[TipoDocumento] [varchar](50) NULL,
	[CuentaContableDocumento] [varchar](50) NULL,
	[FechaEmision] [date] NULL,
	[FechaVencimiento] [date] NULL,
	[Total] [real] NULL,
	[Saldo] [real] NULL,
	[APagar] [real] NULL,
 CONSTRAINT [PK_PagoDetalle] PRIMARY KEY CLUSTERED 
(
	[IdPagoDetalle] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PagosEstado]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PagosEstado](
	[IdPagosEstado] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
 CONSTRAINT [PK_PagosEstado] PRIMARY KEY CLUSTERED 
(
	[IdPagosEstado] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Parametros]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Parametros](
	[IdParametro] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](100) NULL,
	[Descripcion] [varchar](100) NULL,
	[Valor] [varchar](500) NULL,
	[Estado] [int] NULL,
 CONSTRAINT [PK_Parametros] PRIMARY KEY CLUSTERED 
(
	[IdParametro] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PasarelaPago]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasarelaPago](
	[IdPasarela] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
	[Protocolo] [varchar](50) NULL,
	[Ambiente] [varchar](2000) NULL,
	[TipoDocumento] [varchar](50) NULL,
	[CuentaContable] [varchar](100) NULL,
	[Logo] [varchar](200) NULL,
	[Estado] [int] NULL,
	[MonedaPasarela] [varchar](50) NULL,
	[UsuarioSoftlandPay] [varchar](60) NULL,
	[ClaveSoftlandPay] [varchar](100) NULL,
	[EmpresaSoftlandPay] [varchar](50) NULL,
	[CodigoMedioPagoSoftlandPay] [varchar](50) NULL,
	[ManejaAtributos] [int] NULL,
	[ManejaAuxiliar] [int] NULL,
	[EsProduccion] [int] NULL,
	[AmbienteConsultarPago] [varchar](2000) NULL,
	[CodigoComercio] [varchar](max) NULL,
	[SecretKey] [varchar](max) NULL,
 CONSTRAINT [PK_PasarelasPago] PRIMARY KEY CLUSTERED 
(
	[IdPasarela] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PasarelaPagoLog]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasarelaPagoLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdPago] [int] NULL,
	[IdPasarela] [int] NULL,
	[Fecha] [datetime] NULL,
	[Monto] [decimal](18, 0) NULL,
	[Token] [varchar](250) NULL,
	[Codigo] [varchar](100) NULL,
	[Estado] [varchar](100) NULL,
	[OrdenCompra] [varchar](100) NULL,
	[MedioPago] [varchar](100) NULL,
	[Cuotas] [int] NULL,
	[Tarjeta] [varchar](100) NULL,
	[Url] [varchar](500) NULL,
 CONSTRAINT [PK_LogTBK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Perfil]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Perfil](
	[IdPerfil] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](100) NULL,
	[Descripcion] [varchar](100) NULL,
 CONSTRAINT [PK_Perfil] PRIMARY KEY CLUSTERED 
(
	[IdPerfil] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permisos]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permisos](
	[IdPermiso] [int] IDENTITY(1,1) NOT NULL,
	[IdPerfil] [int] NULL,
	[IdAcceso] [int] NULL,
	[Modificar] [int] NULL,
	[Consultar] [int] NULL,
	[Actualizar] [int] NULL,
	[Insertar] [int] NULL,
 CONSTRAINT [PK_Permisos] PRIMARY KEY CLUSTERED 
(
	[IdPermiso] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreguntasFrecuentes]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreguntasFrecuentes](
	[IdPregunta] [int] IDENTITY(1,1) NOT NULL,
	[Pregunta] [varchar](1000) NULL,
	[Respuesta] [varchar](1000) NULL,
	[Estado] [int] NULL,
 CONSTRAINT [PK_PreguntasFrecuentes] PRIMARY KEY CLUSTERED 
(
	[IdPregunta] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RegistroEnvioCorreo]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegistroEnvioCorreo](
	[IdRegistro] [int] IDENTITY(1,1) NOT NULL,
	[IdTipoEnvio] [int] NULL,
	[FechaEnvio] [date] NULL,
	[HoraEnvio] [varchar](50) NULL,
	[IdUsuario] [int] NULL,
	[IdCliente] [int] NULL,
 CONSTRAINT [PK_RegistroEnvioCorreo] PRIMARY KEY CLUSTERED 
(
	[IdRegistro] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoAutomatizacion]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoAutomatizacion](
	[IdTipo] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](100) NULL,
	[EsPosterior] [int] NULL,
	[Estado] [int] NULL,
 CONSTRAINT [PK_TipoAutomatizacion] PRIMARY KEY CLUSTERED 
(
	[IdTipo] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoCobranza]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoCobranza](
	[IdTipoCobranza] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
	[Descripcion] [varchar](500) NULL,
 CONSTRAINT [PK_TipoCobranza] PRIMARY KEY CLUSTERED 
(
	[IdTipoCobranza] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoEnvio]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoEnvio](
	[idTipo] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
	[Estado] [int] NULL,
 CONSTRAINT [PK_TipoEnvio] PRIMARY KEY CLUSTERED 
(
	[idTipo] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoPago]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoPago](
	[IdTipoPago] [int] NOT NULL,
	[Nombre] [varchar](100) NULL,
	[Estado] [int] NULL,
	[TipoDocumento] [varchar](50) NULL,
	[CuentaContable] [varchar](50) NULL,
	[MuestraMonto] [int] NULL,
	[MuestraBanco] [int] NULL,
	[MuestraSerie] [int] NULL,
	[MuestraFecha] [int] NULL,
	[MuestraComprobante] [int] NULL,
	[MuestraCantidad] [int] NULL,
	[GeneraDTE] [int] NULL,
	[FechaMod] [datetime] NULL,
	[IdUsuarioMod] [int] NULL,
 CONSTRAINT [PK_TipoPago] PRIMARY KEY CLUSTERED 
(
	[IdTipoPago] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 29-09-2023 20:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[IdUsuario] [int] IDENTITY(1,1) NOT NULL,
	[Nombres] [varchar](100) NULL,
	[Apellidos] [varchar](100) NULL,
	[Email] [varchar](100) NULL,
	[Password] [varchar](100) NULL,
	[Activo] [int] NULL,
	[IdPerfil] [int] NULL,
	[FechaCreacion] [datetime] NULL,
	[FechaEnvioValidacion] [datetime] NULL,
	[CuentaActivada] [int] NULL,
 CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED 
(
	[IdUsuario] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Acceso] ON 
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (1, N'Dashboard', NULL, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (2, N'Portal', NULL, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (3, N'Administración', NULL, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (4, N'Mi Perfil', 2, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (5, N'Mis Compras', 2, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (6, N'Estado de cuentas', 2, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (7, N'Mi Dashboard', 2, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (8, N'Generar Acceso Cliente', 3, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (9, N'Configuración Correo', 3, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (10, N'Configuración Portal', 3, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (11, N'Dashboard Administrador', 1, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (12, N'Perfil Administrador', 3, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (13, N'Configuración de Pagos', 3, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (14, N'Menu Cobranzas', NULL, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (15, N'Cobranzas', 14, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (16, N'Clientes Excluidos', 14, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (17, N'Automatizaciones', 14, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (18, N'Perfiles', 3, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (19, N'Permisos', 3, 1)
GO
INSERT [dbo].[Acceso] ([IdAcceso], [Nombre], [MenuPadre], [Activo]) VALUES (20, N'Usuarios', 3, 1)
GO
SET IDENTITY_INSERT [dbo].[Acceso] OFF
GO
SET IDENTITY_INSERT [dbo].[CobranzaHorarios] ON 
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (1, N'00:00', 0, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (2, N'01:00', 1, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (3, N'02:00', 2, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (4, N'03:00', 3, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (5, N'04:00', 4, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (6, N'05:00', 5, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (7, N'06:00', 6, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (8, N'07:00', 7, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (9, N'08:00', 8, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (10, N'09:00', 9, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (11, N'10:00', 10, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (12, N'11:00', 11, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (13, N'12:00', 12, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (14, N'13:00', 13, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (15, N'14:00', 14, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (16, N'15:00', 15, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (17, N'16:00', 16, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (18, N'17:00', 17, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (19, N'18:00', 18, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (20, N'19:00', 19, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (21, N'20:00', 20, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (22, N'21:00', 21, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (23, N'22:00', 22, 0)
GO
INSERT [dbo].[CobranzaHorarios] ([IdHorario], [Horario], [Hora], [Minuto]) VALUES (24, N'23:00', 23, 0)
GO
SET IDENTITY_INSERT [dbo].[CobranzaHorarios] OFF
GO
SET IDENTITY_INSERT [dbo].[CobranzaPeriocidad] ON 
GO
INSERT [dbo].[CobranzaPeriocidad] ([IdPeriocidad], [Nombre], [DiaMes], [Estado]) VALUES (1, N'Primer día del mes', 1, 1)
GO
INSERT [dbo].[CobranzaPeriocidad] ([IdPeriocidad], [Nombre], [DiaMes], [Estado]) VALUES (2, N'Mitad de mes (Día 15)', 15, 1)
GO
INSERT [dbo].[CobranzaPeriocidad] ([IdPeriocidad], [Nombre], [DiaMes], [Estado]) VALUES (3, N'Ultimo día del mes', 30, 1)
GO
INSERT [dbo].[CobranzaPeriocidad] ([IdPeriocidad], [Nombre], [DiaMes], [Estado]) VALUES (4, N'Especificar día', -1, 1)
GO
INSERT [dbo].[CobranzaPeriocidad] ([IdPeriocidad], [Nombre], [DiaMes], [Estado]) VALUES (5, N'Semanal', -1, 1)
GO
SET IDENTITY_INSERT [dbo].[CobranzaPeriocidad] OFF
GO
SET IDENTITY_INSERT [dbo].[ConfiguracionCorreoCasillas] ON 
GO
INSERT [dbo].[ConfiguracionCorreoCasillas] ([IdCasilla], [Casilla], [CantidadDia]) VALUES (1, N'Outlook.cl', 300)
GO
INSERT [dbo].[ConfiguracionCorreoCasillas] ([IdCasilla], [Casilla], [CantidadDia]) VALUES (2, N'Gmail.com', 500)
GO
INSERT [dbo].[ConfiguracionCorreoCasillas] ([IdCasilla], [Casilla], [CantidadDia]) VALUES (3, N'dteinnova.cl', 300)
GO
INSERT [dbo].[ConfiguracionCorreoCasillas] ([IdCasilla], [Casilla], [CantidadDia]) VALUES (4, N'softland.cl', 1500)
GO
SET IDENTITY_INSERT [dbo].[ConfiguracionCorreoCasillas] OFF
GO
INSERT [dbo].[EstadoCobranza] ([IdEstadoCobranza], [Nombre]) VALUES (1, N'PENDIENTE')
GO
INSERT [dbo].[EstadoCobranza] ([IdEstadoCobranza], [Nombre]) VALUES (2, N'PARCIALMENTE ENVIADA')
GO
INSERT [dbo].[EstadoCobranza] ([IdEstadoCobranza], [Nombre]) VALUES (3, N'ENVIADA')
GO
INSERT [dbo].[EstadoCobranza] ([IdEstadoCobranza], [Nombre]) VALUES (4, N'PARCIALMENTE PAGADA')
GO
INSERT [dbo].[EstadoCobranza] ([IdEstadoCobranza], [Nombre]) VALUES (5, N'PAGADA')
GO

SET IDENTITY_INSERT [dbo].[PagosEstado] ON 
GO
INSERT [dbo].[PagosEstado] ([IdPagosEstado], [Nombre]) VALUES (1, N'PENDIENTE')
GO
INSERT [dbo].[PagosEstado] ([IdPagosEstado], [Nombre]) VALUES (2, N'PAGADO')
GO
INSERT [dbo].[PagosEstado] ([IdPagosEstado], [Nombre]) VALUES (3, N'ANULADO')
GO
INSERT [dbo].[PagosEstado] ([IdPagosEstado], [Nombre]) VALUES (4, N'PAGADO NO GENERA COMPROBANTE')
GO
SET IDENTITY_INSERT [dbo].[PagosEstado] OFF
GO
SET IDENTITY_INSERT [dbo].[PasarelaPago] ON 
GO
INSERT [dbo].[PasarelaPago] ([IdPasarela], [Nombre], [Protocolo], [Ambiente], [TipoDocumento], [CuentaContable], [Logo], [Estado], [MonedaPasarela], [UsuarioSoftlandPay], [ClaveSoftlandPay], [EmpresaSoftlandPay], [CodigoMedioPagoSoftlandPay], [ManejaAtributos], [ManejaAuxiliar], [EsProduccion], [AmbienteConsultarPago], [CodigoComercio], [SecretKey]) VALUES (1, N'WebPay', N'https://', N'VW/VWTransbankGenerarPago?url_redireccion={REDIRECCION}&url_callback={CALLBACK}&id_interno={IDINTERNO}&monto_total={TOTAL}&monto_bruto={BRUTO}&rutCliente={RUT}&tipo={TIPO}&monto_impuestos={IMPUESTO}&nombre_cliente={NOMBRE}&apellido_cliente={APELLIDO}&correo_cliente={CORREO}&esProductivo={ESPRODUCTIVO}', N'', N'', N'assets/images/pasarelas/Webpay.png', 1, N'CLP', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'', N'', N'')
GO
INSERT [dbo].[PasarelaPago] ([IdPasarela], [Nombre], [Protocolo], [Ambiente], [TipoDocumento], [CuentaContable], [Logo], [Estado], [MonedaPasarela], [UsuarioSoftlandPay], [ClaveSoftlandPay], [EmpresaSoftlandPay], [CodigoMedioPagoSoftlandPay], [ManejaAtributos], [ManejaAuxiliar], [EsProduccion], [AmbienteConsultarPago], [CodigoComercio], [SecretKey]) VALUES (2, N'Mercado Pago', N'https://', N'INTEGRACION', NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL)
GO
INSERT [dbo].[PasarelaPago] ([IdPasarela], [Nombre], [Protocolo], [Ambiente], [TipoDocumento], [CuentaContable], [Logo], [Estado], [MonedaPasarela], [UsuarioSoftlandPay], [ClaveSoftlandPay], [EmpresaSoftlandPay], [CodigoMedioPagoSoftlandPay], [ManejaAtributos], [ManejaAuxiliar], [EsProduccion], [AmbienteConsultarPago], [CodigoComercio], [SecretKey]) VALUES (3, N'Flow', N'https://', N'https://sandbox.flow.cl/api', NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL)
GO
INSERT [dbo].[PasarelaPago] ([IdPasarela], [Nombre], [Protocolo], [Ambiente], [TipoDocumento], [CuentaContable], [Logo], [Estado], [MonedaPasarela], [UsuarioSoftlandPay], [ClaveSoftlandPay], [EmpresaSoftlandPay], [CodigoMedioPagoSoftlandPay], [ManejaAtributos], [ManejaAuxiliar], [EsProduccion], [AmbienteConsultarPago], [CodigoComercio], [SecretKey]) VALUES (4, N'Fpay', N'https://', NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL)
GO
INSERT [dbo].[PasarelaPago] ([IdPasarela], [Nombre], [Protocolo], [Ambiente], [TipoDocumento], [CuentaContable], [Logo], [Estado], [MonedaPasarela], [UsuarioSoftlandPay], [ClaveSoftlandPay], [EmpresaSoftlandPay], [CodigoMedioPagoSoftlandPay], [ManejaAtributos], [ManejaAuxiliar], [EsProduccion], [AmbienteConsultarPago], [CodigoComercio], [SecretKey]) VALUES (5, N'Vpos', N'https://', N'VW/VirtualPosGenerarPago?url_redireccion={REDIRECCION}&url_callback={CALLBACK}&id_interno={IDINTERNO}&monto_total={TOTAL}&monto_bruto={BRUTO}&rutCliente={RUT}&tipo={TIPO}&monto_impuestos={IMPUESTO}&nombre_cliente={NOMBRE}&apellido_cliente={APELLIDO}&correo_cliente={CORREO}&esProductivo={ESPRODUCTIVO}', N'', N'', N'assets/images/pasarelas/logo-vpos.png', 1, N'CLP', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'VW/VWVirtualPosObtenerEstadoPago?id_transaccion={ID}&esProductivo={ESPRODUCTIVO}', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[PasarelaPago] OFF
GO
SET IDENTITY_INSERT [dbo].[Perfil] ON 
GO
INSERT [dbo].[Perfil] ([IdPerfil], [Nombre], [Descripcion]) VALUES (1, N'Administrador', N'perfil con permisos de administrador')
GO
INSERT [dbo].[Perfil] ([IdPerfil], [Nombre], [Descripcion]) VALUES (2, N'Cliente', N'Clienter')
GO
SET IDENTITY_INSERT [dbo].[Perfil] OFF
GO
SET IDENTITY_INSERT [dbo].[Permisos] ON 
GO
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (1, 1, 1, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (2, 1, 3, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (3, 1, 8, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (4, 1, 9, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (5, 1, 10, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (6, 1, 11, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (7, 1, 12, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (8, 1, 13, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (9, 1, 14, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (10, 1, 15, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (11, 1, 16, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (12, 1, 17, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (13, 1, 18, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (14, 1, 19, NULL, NULL, NULL, NULL)
GO																															   
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (15, 1, 20, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (16, 2, 4, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (17, 2, 5, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (18, 2, 6, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (19, 2, 7, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Permisos] ([IdPermiso], [IdPerfil], [IdAcceso], [Modificar], [Consultar], [Actualizar], [Insertar]) VALUES (20, 2, 2, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Permisos] OFF
GO
SET IDENTITY_INSERT [dbo].[TipoAutomatizacion] ON 
GO
INSERT [dbo].[TipoAutomatizacion] ([IdTipo], [Nombre], [EsPosterior], [Estado]) VALUES (1, N'Recordatorio', -1, 1)
GO
INSERT [dbo].[TipoAutomatizacion] ([IdTipo], [Nombre], [EsPosterior], [Estado]) VALUES (2, N'Estado de Cuenta', 0, 1)
GO
INSERT [dbo].[TipoAutomatizacion] ([IdTipo], [Nombre], [EsPosterior], [Estado]) VALUES (3, N'Cobranza', 1, 1)
GO
SET IDENTITY_INSERT [dbo].[TipoAutomatizacion] OFF
GO
SET IDENTITY_INSERT [dbo].[TipoCobranza] ON 
GO
INSERT [dbo].[TipoCobranza] ([IdTipoCobranza], [Nombre], [Descripcion]) VALUES (1, N'Cobranza', N'Se enviarán los documentos vencidos según configuración ingresada')
GO
INSERT [dbo].[TipoCobranza] ([IdTipoCobranza], [Nombre], [Descripcion]) VALUES (2, N'PreCobranza', N'Se enviarán los documentos próximos a vencer según configuración ingresada')
GO
SET IDENTITY_INSERT [dbo].[TipoCobranza] OFF
GO
SET IDENTITY_INSERT [dbo].[TipoEnvio] ON 
GO
INSERT [dbo].[TipoEnvio] ([idTipo], [Nombre], [Estado]) VALUES (1, N'Acceso', 1)
GO
INSERT [dbo].[TipoEnvio] ([idTipo], [Nombre], [Estado]) VALUES (2, N'Pago Portal', 1)
GO
INSERT [dbo].[TipoEnvio] ([idTipo], [Nombre], [Estado]) VALUES (3, N'Pago Rapido', 1)
GO
INSERT [dbo].[TipoEnvio] ([idTipo], [Nombre], [Estado]) VALUES (4, N'Recuperar Clave', 1)
GO
INSERT [dbo].[TipoEnvio] ([idTipo], [Nombre], [Estado]) VALUES (5, N'Envio Documento', 1)
GO
INSERT [dbo].[TipoEnvio] ([idTipo], [Nombre], [Estado]) VALUES (6, N'Envio Cobranza', 1)
GO
SET IDENTITY_INSERT [dbo].[TipoEnvio] OFF
GO
SET IDENTITY_INSERT [dbo].[Usuarios] ON 
GO
SET IDENTITY_INSERT [dbo].[Usuarios] OFF
GO
ALTER TABLE [dbo].[Automatizacion]  WITH CHECK ADD  CONSTRAINT [FK_Automatizacion_CobranzaHorarios] FOREIGN KEY([IdHorario])
REFERENCES [dbo].[CobranzaHorarios] ([IdHorario])
GO
ALTER TABLE [dbo].[Automatizacion] CHECK CONSTRAINT [FK_Automatizacion_CobranzaHorarios]
GO
ALTER TABLE [dbo].[Automatizacion]  WITH CHECK ADD  CONSTRAINT [FK_Automatizacion_CobranzaPeriocidad] FOREIGN KEY([IdPerioricidad])
REFERENCES [dbo].[CobranzaPeriocidad] ([IdPeriocidad])
GO
ALTER TABLE [dbo].[Automatizacion] CHECK CONSTRAINT [FK_Automatizacion_CobranzaPeriocidad]
GO
ALTER TABLE [dbo].[Automatizacion]  WITH NOCHECK ADD  CONSTRAINT [FK_Automatizacion_TipoAutomatizacion] FOREIGN KEY([IdTipoAutomatizacion])
REFERENCES [dbo].[TipoAutomatizacion] ([IdTipo])
GO
ALTER TABLE [dbo].[Automatizacion] CHECK CONSTRAINT [FK_Automatizacion_TipoAutomatizacion]
GO
ALTER TABLE [dbo].[CobranzaCabecera]  WITH CHECK ADD  CONSTRAINT [FK_CobranzaCabecera_EstadoCobranza] FOREIGN KEY([IdEstado])
REFERENCES [dbo].[EstadoCobranza] ([IdEstadoCobranza])
GO
ALTER TABLE [dbo].[CobranzaCabecera] CHECK CONSTRAINT [FK_CobranzaCabecera_EstadoCobranza]
GO
ALTER TABLE [dbo].[CobranzaCabecera]  WITH CHECK ADD  CONSTRAINT [FK_CobranzaCabecera_TipoCobranza] FOREIGN KEY([IdTipoCobranza])
REFERENCES [dbo].[TipoCobranza] ([IdTipoCobranza])
GO
ALTER TABLE [dbo].[CobranzaCabecera] CHECK CONSTRAINT [FK_CobranzaCabecera_TipoCobranza]
GO
ALTER TABLE [dbo].[CobranzaDetalle]  WITH CHECK ADD  CONSTRAINT [FK_CobranzaDetalle_CobranzaCabecera] FOREIGN KEY([IdCobranza])
REFERENCES [dbo].[CobranzaCabecera] ([IdCobranza])
GO
ALTER TABLE [dbo].[CobranzaDetalle] CHECK CONSTRAINT [FK_CobranzaDetalle_CobranzaCabecera]
GO
ALTER TABLE [dbo].[CobranzaDetalle]  WITH CHECK ADD  CONSTRAINT [FK_CobranzaDetalle_EstadoCobranza] FOREIGN KEY([IdEstado])
REFERENCES [dbo].[EstadoCobranza] ([IdEstadoCobranza])
GO
ALTER TABLE [dbo].[CobranzaDetalle] CHECK CONSTRAINT [FK_CobranzaDetalle_EstadoCobranza]
GO
ALTER TABLE [dbo].[PagosCabecera]  WITH CHECK ADD  CONSTRAINT [FK_PagosCabecera_ClientesPortal] FOREIGN KEY([IdCliente])
REFERENCES [dbo].[ClientesPortal] ([IdCliente])
GO
ALTER TABLE [dbo].[PagosCabecera] CHECK CONSTRAINT [FK_PagosCabecera_ClientesPortal]
GO
ALTER TABLE [dbo].[PagosCabecera]  WITH CHECK ADD  CONSTRAINT [FK_PagosCabecera_PagosEstado] FOREIGN KEY([IdPagoEstado])
REFERENCES [dbo].[PagosEstado] ([IdPagosEstado])
GO
ALTER TABLE [dbo].[PagosCabecera] CHECK CONSTRAINT [FK_PagosCabecera_PagosEstado]
GO
ALTER TABLE [dbo].[PagosDetalle]  WITH CHECK ADD  CONSTRAINT [FK_PagosDetalle_PagosCabecera] FOREIGN KEY([IdPago])
REFERENCES [dbo].[PagosCabecera] ([IdPago])
GO
ALTER TABLE [dbo].[PagosDetalle] CHECK CONSTRAINT [FK_PagosDetalle_PagosCabecera]
GO
ALTER TABLE [dbo].[PasarelaPagoLog]  WITH CHECK ADD  CONSTRAINT [FK_PasarelaPagoLog_PagosCabecera] FOREIGN KEY([IdPago])
REFERENCES [dbo].[PagosCabecera] ([IdPago])
GO
ALTER TABLE [dbo].[PasarelaPagoLog] CHECK CONSTRAINT [FK_PasarelaPagoLog_PagosCabecera]
GO
ALTER TABLE [dbo].[PasarelaPagoLog]  WITH CHECK ADD  CONSTRAINT [FK_PasarelaPagoLog_PasarelaPago] FOREIGN KEY([IdPasarela])
REFERENCES [dbo].[PasarelaPago] ([IdPasarela])
GO
ALTER TABLE [dbo].[PasarelaPagoLog] CHECK CONSTRAINT [FK_PasarelaPagoLog_PasarelaPago]
GO
ALTER TABLE [dbo].[Permisos]  WITH CHECK ADD  CONSTRAINT [FK_Permisos_Acceso] FOREIGN KEY([IdAcceso])
REFERENCES [dbo].[Acceso] ([IdAcceso])
GO
ALTER TABLE [dbo].[Permisos] CHECK CONSTRAINT [FK_Permisos_Acceso]
GO
ALTER TABLE [dbo].[Permisos]  WITH CHECK ADD  CONSTRAINT [FK_Permisos_Perfil] FOREIGN KEY([IdPerfil])
REFERENCES [dbo].[Perfil] ([IdPerfil])
GO
ALTER TABLE [dbo].[Permisos] CHECK CONSTRAINT [FK_Permisos_Perfil]
GO
ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_Usuarios_Perfil] FOREIGN KEY([IdPerfil])
REFERENCES [dbo].[Perfil] ([IdPerfil])
GO
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_Perfil]
GO
