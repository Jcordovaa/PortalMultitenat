using System;
using System.Collections.Generic;
using ApiPortal.Security.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class PortalClientesSoftlandContext : DbContext
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public PortalClientesSoftlandContext()
        {
        }

        public PortalClientesSoftlandContext(DbContextOptions<PortalClientesSoftlandContext> options, IHttpContextAccessor contextAccessor)
            : base(options)
        {
            _contextAccessor = contextAccessor;
        }

        public PortalClientesSoftlandContext(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public virtual DbSet<Acceso> Accesos { get; set; } = null!;
        public virtual DbSet<ApiSoftland> ApiSoftlands { get; set; } = null!;
        public virtual DbSet<Automatizacion> Automatizacions { get; set; } = null!;
        public virtual DbSet<ClientesExcluido> ClientesExcluidos { get; set; } = null!;
        public virtual DbSet<ClientesPortal> ClientesPortals { get; set; } = null!;
        public virtual DbSet<CobranzaCabecera> CobranzaCabeceras { get; set; } = null!;
        public virtual DbSet<CobranzaDetalle> CobranzaDetalles { get; set; } = null!;
        public virtual DbSet<CobranzaHorario> CobranzaHorarios { get; set; } = null!;
        public virtual DbSet<CobranzaPeriocidad> CobranzaPeriocidads { get; set; } = null!;
        public virtual DbSet<ConfigracionCobranza> ConfigracionCobranzas { get; set; } = null!;
        public virtual DbSet<ConfiguracionCorreo> ConfiguracionCorreos { get; set; } = null!;
        public virtual DbSet<ConfiguracionCorreoCasilla> ConfiguracionCorreoCasillas { get; set; } = null!;
        public virtual DbSet<ConfiguracionDiseno> ConfiguracionDisenos { get; set; } = null!;
        public virtual DbSet<ConfiguracionEmpresa> ConfiguracionEmpresas { get; set; } = null!;
        public virtual DbSet<ConfiguracionPagoCliente> ConfiguracionPagoClientes { get; set; } = null!;
        public virtual DbSet<ConfiguracionPortal> ConfiguracionPortals { get; set; } = null!;
        public virtual DbSet<ConfiguracionTiposDocumento> ConfiguracionTiposDocumentos { get; set; } = null!;
        public virtual DbSet<CorreoDte> CorreoDtes { get; set; } = null!;
        public virtual DbSet<EstadoCobranza> EstadoCobranzas { get; set; } = null!;
        public virtual DbSet<Feriado> Feriados { get; set; } = null!;
        public virtual DbSet<LogApi> LogApis { get; set; } = null!;
        public virtual DbSet<LogApiDetalle> LogApiDetalles { get; set; } = null!;
        public virtual DbSet<LogCobranza> LogCobranzas { get; set; } = null!;
        public virtual DbSet<LogCorreo> LogCorreos { get; set; } = null!;
        public virtual DbSet<LogProceso> LogProcesos { get; set; } = null!;
        public virtual DbSet<LogSoftlandpay> LogSoftlandpays { get; set; } = null!;
        public virtual DbSet<PagosCabecera> PagosCabeceras { get; set; } = null!;
        public virtual DbSet<PagosDetalle> PagosDetalles { get; set; } = null!;
        public virtual DbSet<PagosEstado> PagosEstados { get; set; } = null!;
        public virtual DbSet<Parametro> Parametros { get; set; } = null!;
        public virtual DbSet<PasarelaPago> PasarelaPagos { get; set; } = null!;
        public virtual DbSet<PasarelaPagoLog> PasarelaPagoLogs { get; set; } = null!;
        public virtual DbSet<Perfil> Perfils { get; set; } = null!;
        public virtual DbSet<Permiso> Permisos { get; set; } = null!;
        public virtual DbSet<PreguntasFrecuente> PreguntasFrecuentes { get; set; } = null!;
        public virtual DbSet<RegistroEnvioCorreo> RegistroEnvioCorreos { get; set; } = null!;
        public virtual DbSet<TipoAutomatizacion> TipoAutomatizacions { get; set; } = null!;
        public virtual DbSet<TipoCobranza> TipoCobranzas { get; set; } = null!;
        public virtual DbSet<TipoEnvio> TipoEnvios { get; set; } = null!;
        public virtual DbSet<TipoPago> TipoPagos { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var currentTenant = _contextAccessor.HttpContext?.GetTenant();
                optionsBuilder.UseSqlServer(currentTenant.Items["ConnectionString"].ToString());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acceso>(entity =>
            {
                entity.HasKey(e => e.IdAcceso);

                entity.ToTable("Acceso");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ApiSoftland>(entity =>
            {
                entity.ToTable("ApiSoftland");

                entity.Property(e => e.ActualizaCliente)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Ambiente)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AreaDatos)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CadenaAlmacenamientoAzure)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.CapturaComprobantes)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConsultaAuxiliar)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConsultaCargos)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConsultaCliente)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConsultaComunas)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConsultaGiros)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConsultaMonedas)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConsultaPlanDeCuentas)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConsultaRegiones)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ConsultaTiposDeDocumentos)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ContabilizaPagos)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ContactosXauxiliar)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("ContactosXAuxiliar");

                entity.Property(e => e.CuentasPasarelaPagos)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DespachoDeDocumento)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DetalleDocumento)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DetalleNotaDeVenta)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DocContabilizadosResumenxRut)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DocumentosContabilizados)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DocumentosContabilizadosResumen)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.DocumentosFacturados)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Login)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("login");

                entity.Property(e => e.ObtenerPdf)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ObtenerPdfDocumento)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ObtieneCategoriaClientes)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ObtieneCondicionesVenta)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ObtieneGuiasPendientes)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ObtieneListasPrecio)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ObtieneModulos)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ObtieneVendedores)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PagosxDocumento)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PendientesPorFacturar)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ProductosComprados)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ResumenContable)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Token)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TopDeudores)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TransbankRegistrarCliente)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.UrlAlmacenamientoArchivos)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Automatizacion>(entity =>
            {
                entity.HasKey(e => e.IdAutomatizacion);

                entity.ToTable("Automatizacion");

                entity.Property(e => e.CodCanalVenta).IsUnicode(false);

                entity.Property(e => e.CodCargo).IsUnicode(false);

                entity.Property(e => e.CodCategoriaCliente).IsUnicode(false);

                entity.Property(e => e.CodCobrador).IsUnicode(false);

                entity.Property(e => e.CodCondicionVenta).IsUnicode(false);

                entity.Property(e => e.CodListaPrecios).IsUnicode(false);

                entity.Property(e => e.CodVendedor).IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.TipoDocumentos)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdHorarioNavigation)
                    .WithMany(p => p.Automatizacions)
                    .HasForeignKey(d => d.IdHorario)
                    .HasConstraintName("FK_Automatizacion_CobranzaHorarios");

                entity.HasOne(d => d.IdPerioricidadNavigation)
                    .WithMany(p => p.Automatizacions)
                    .HasForeignKey(d => d.IdPerioricidad)
                    .HasConstraintName("FK_Automatizacion_CobranzaPeriocidad");

                entity.HasOne(d => d.IdTipoAutomatizacionNavigation)
                    .WithMany(p => p.Automatizacions)
                    .HasForeignKey(d => d.IdTipoAutomatizacion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Automatizacion_TipoAutomatizacion");
            });

            modelBuilder.Entity<ClientesExcluido>(entity =>
            {
                entity.HasKey(e => e.IdExcluido)
                    .HasName("PK_AlumnosExcluidos");

                entity.Property(e => e.CodAuxCliente)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NombreCliente)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.RutCliente)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ClientesPortal>(entity =>
            {
                entity.HasKey(e => e.IdCliente);

                entity.ToTable("ClientesPortal");

                entity.Property(e => e.Clave)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CodAux)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Correo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Rut)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CobranzaCabecera>(entity =>
            {
                entity.HasKey(e => e.IdCobranza);

                entity.ToTable("CobranzaCabecera");

                entity.Property(e => e.CanalesVenta).IsUnicode(false);

                entity.Property(e => e.CargosContactos).IsUnicode(false);

                entity.Property(e => e.CategoriaCliente).IsUnicode(false);

                entity.Property(e => e.Cobradores).IsUnicode(false);

                entity.Property(e => e.CondicionVenta).IsUnicode(false);

                entity.Property(e => e.DiaSemanaEnvio)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaCreacion).HasColumnType("date");

                entity.Property(e => e.FechaDesde).HasColumnType("date");

                entity.Property(e => e.FechaFin).HasColumnType("date");

                entity.Property(e => e.FechaHasta).HasColumnType("date");

                entity.Property(e => e.FechaInicio).HasColumnType("date");

                entity.Property(e => e.HoraCreacion)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ListaPrecio).IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.TipoDocumento).IsUnicode(false);

                entity.Property(e => e.Vendedor).IsUnicode(false);

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.CobranzaCabeceras)
                    .HasForeignKey(d => d.IdEstado)
                    .HasConstraintName("FK_CobranzaCabecera_EstadoCobranza");

                entity.HasOne(d => d.IdTipoCobranzaNavigation)
                    .WithMany(p => p.CobranzaCabeceras)
                    .HasForeignKey(d => d.IdTipoCobranza)
                    .HasConstraintName("FK_CobranzaCabecera_TipoCobranza");
            });

            modelBuilder.Entity<CobranzaDetalle>(entity =>
            {
                entity.HasKey(e => e.IdCobranzaDetalle);

                entity.ToTable("CobranzaDetalle");

                entity.Property(e => e.CodAuxCliente)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ComprobanteContable)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CuentaContable)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmailCliente)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.FechaEmision).HasColumnType("date");

                entity.Property(e => e.FechaEnvio).HasColumnType("date");

                entity.Property(e => e.FechaPago).HasColumnType("date");

                entity.Property(e => e.FechaVencimiento).HasColumnType("date");

                entity.Property(e => e.FolioDte)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("FolioDTE");

                entity.Property(e => e.HoraEnvio)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HoraPago)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NombreCliente)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.RutCliente)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TipoDocumento)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCobranzaNavigation)
                    .WithMany(p => p.CobranzaDetalles)
                    .HasForeignKey(d => d.IdCobranza)
                    .HasConstraintName("FK_CobranzaDetalle_CobranzaCabecera");

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.CobranzaDetalles)
                    .HasForeignKey(d => d.IdEstado)
                    .HasConstraintName("FK_CobranzaDetalle_EstadoCobranza");
            });

            modelBuilder.Entity<CobranzaHorario>(entity =>
            {
                entity.HasKey(e => e.IdHorario);

                entity.Property(e => e.Horario)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CobranzaPeriocidad>(entity =>
            {
                entity.HasKey(e => e.IdPeriocidad);

                entity.ToTable("CobranzaPeriocidad");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ConfigracionCobranza>(entity =>
            {
                entity.HasKey(e => e.IdConfigCobranza);

                entity.Property(e => e.CondicionesCredito).IsUnicode(false);

                entity.Property(e => e.TipoDocumentoCobranza).IsUnicode(false);
            });

            modelBuilder.Entity<ConfiguracionCorreo>(entity =>
            {
                entity.HasKey(e => e.IdConfiguracionCorreo);

                entity.ToTable("ConfiguracionCorreo");

                entity.Property(e => e.AsuntoAccesoCliente)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.AsuntoAvisoPagoCliente)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.AsuntoCambioClave)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.AsuntoCambioCorreo)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.AsuntoCambioDatos)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.AsuntoCobranza)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.AsuntoEnvioDocumentos)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.AsuntoEstadoCuenta)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.AsuntoPagoCliente)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.AsuntoPagoSinComprobante)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.AsuntoPreCobranza)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.AsuntoRecuperarClave)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.Clave)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ColorBoton)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CorreoAvisoPago)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.CorreoOrigen)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LogoCorreo)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NombreCorreos)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SmtpServer)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Ssl).HasColumnName("SSL");

                entity.Property(e => e.TextoAccesoCliente)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoAvisoPagoCliente)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoCambioClave)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoCambioCorreo)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoCambioDatos)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoCobranza)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoEnvioDocumentos)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoEstadoCuenta)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoMensajeActivacion)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoPagoCliente)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoPagoSinComprobante)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoPreCobranza)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TextoRecuperarClave)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TituloAccesoCliente)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.TituloAvisoPagoCliente)
                    .HasMaxLength(1700)
                    .IsUnicode(false);

                entity.Property(e => e.TituloCambioClave)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.TituloCambioCorreo)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.TituloCambioDatos)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.TituloCobranza)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.TituloEnvioDocumentos)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.TituloEstadoCuenta)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.TituloPagoCliente)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.TituloPagoSinComprobante)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.TituloPreCobranza)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.TituloRecuperarClave)
                    .HasMaxLength(170)
                    .IsUnicode(false);

                entity.Property(e => e.Usuario)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UtilizaSes).HasColumnName("UtilizaSES");
            });

            modelBuilder.Entity<ConfiguracionCorreoCasilla>(entity =>
            {
                entity.HasKey(e => e.IdCasilla);

                entity.Property(e => e.Casilla)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ConfiguracionDiseno>(entity =>
            {
                entity.HasKey(e => e.IdConfiguracionTextosColores);

                entity.ToTable("ConfiguracionDiseno");

                entity.Property(e => e.BannerMisCompras)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.BannerPagoRapido)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BannerPortal)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ColorBotonBuscar)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorBotonCancelarModalPerfil)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorBotonClavePerfil)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorBotonEstadoPerfil)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorBotonGuardarModalPerfil)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorBotonInicioSesion)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorBotonModificarPerfil)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorBotonPagar)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorBotonPagoRapido)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorBotonUltimasCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorFondoDocumentos)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorFondoGuiasMisCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorFondoMisCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorFondoPendientesMisCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorFondoPorVencer)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorFondoPortada)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorFondoProductosMisCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorFondoResumenContable)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorFondoUltimasCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorFondoVencidos)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorHoverBotonUltimasCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorHoverBotonesPerfil)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorIconoPendientes)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorIconoPorVencer)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorIconoVencidos)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorIconosMisCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorPaginador)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorSeleccionDocumentos)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorTextoBotonUltimasCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorTextoFechaUltimasCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorTextoMisCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorTextoMontoUltimasCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorTextoPendientes)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorTextoPorVencer)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorTextoUltimasCompras)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ColorTextoVencidos)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Favicon)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.IconoClavePerfil)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IconoContactos)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.IconoEditarPerfil)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IconoEstadoPerfil)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IconoMisCompras)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ImagenPortada)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ImagenUltimasCompras)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ImagenUsuario)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LogoMinimalistaSidebar)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LogoPortada)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.LogoSidebar)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TextoCobranzaExpirada).IsUnicode(false);

                entity.Property(e => e.TextoDescargaCobranza).IsUnicode(false);

                entity.Property(e => e.TextoNoConsideraTodaDeuda).IsUnicode(false);

                entity.Property(e => e.TituloComprasFacturadas)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.TituloGuiasPendientes)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.TituloMisCompras)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.TituloMonedaPeso)
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.TituloOtraMoneda)
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.TituloPendientesDashboard)
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.TituloPendientesFacturar)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.TituloPorVencerDashboard)
                    .HasMaxLength(70)
                    .IsUnicode(false);

                entity.Property(e => e.TituloProductos)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.TituloResumenContable)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.TituloUltimasCompras)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.TituloVencidosDashboard)
                    .HasMaxLength(70)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ConfiguracionEmpresa>(entity =>
            {
                entity.HasKey(e => e.IdConfiguracionEmpresa);

                entity.ToTable("ConfiguracionEmpresa");

                entity.Property(e => e.CorreoContacto)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Direccion)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Facebook)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Instagram)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Linkedin)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Logo)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.NombreEmpresa)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RutEmpresa)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RutaGoogleMaps)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Twitter)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UrlPortal)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Web)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Youtube)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ConfiguracionPagoCliente>(entity =>
            {
                entity.HasKey(e => e.IdConfiguracionPago)
                    .HasName("PK_ConfiguracionPago");

                entity.ToTable("ConfiguracionPagoCliente");

                entity.Property(e => e.AreaNegocio)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CentroCosto)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CuentasContablesDeuda).IsUnicode(false);

                entity.Property(e => e.DocumentosCobranza).IsUnicode(false);

                entity.Property(e => e.GlosaComprobante)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.GlosaDetalle)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.GlosaPago)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.MonedaUtilizada)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SegundaMonedaUtilizada)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TiposDocumentosDeuda).IsUnicode(false);
            });

            modelBuilder.Entity<ConfiguracionPortal>(entity =>
            {
                entity.HasKey(e => e.IdConfiguracionPortal);

                entity.ToTable("ConfiguracionPortal");

                entity.Property(e => e.ImagenCabeceraCompras)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ImagenCaberaPerfil)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ConfiguracionTiposDocumento>(entity =>
            {
                entity.HasKey(e => e.IdTipoDocConfig);

                entity.Property(e => e.CodErp)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(60)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CorreoDte>(entity =>
            {
                entity.ToTable("CorreoDTE");

                entity.Property(e => e.Correo)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Rut)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EstadoCobranza>(entity =>
            {
                entity.HasKey(e => e.IdEstadoCobranza);

                entity.ToTable("EstadoCobranza");

                entity.Property(e => e.IdEstadoCobranza).ValueGeneratedNever();

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Feriado>(entity =>
            {
                entity.HasKey(e => e.IdFeriado);

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LogApi>(entity =>
            {
                entity.ToTable("LogApi");

                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Api).IsUnicode(false);

                entity.Property(e => e.Inicio).HasColumnType("datetime");

                entity.Property(e => e.Termino).HasColumnType("datetime");
            });

            modelBuilder.Entity<LogApiDetalle>(entity =>
            {
                entity.ToTable("LogApiDetalle");

                entity.Property(e => e.IdLogApi)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Inicio).HasColumnType("datetime");

                entity.Property(e => e.Metodo).IsUnicode(false);

                entity.Property(e => e.Termino).HasColumnType("datetime");
            });

            modelBuilder.Entity<LogCobranza>(entity =>
            {
                entity.HasKey(e => e.IdLogCobranza);

                entity.Property(e => e.CobranzasConsideradas)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaInicio).HasColumnType("datetime");

                entity.Property(e => e.FechaTermino).HasColumnType("datetime");
            });

            modelBuilder.Entity<LogCorreo>(entity =>
            {
                entity.HasKey(e => e.IdLogEmail);

                entity.Property(e => e.CodAux)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Error).IsUnicode(false);

                entity.Property(e => e.Estado)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.Rut)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tipo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LogProceso>(entity =>
            {
                entity.HasKey(e => e.IdTipoProceso);

                entity.Property(e => e.Excepcion).IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.Property(e => e.Hora)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mensaje).IsUnicode(false);

                entity.Property(e => e.Ruta)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LogSoftlandpay>(entity =>
            {
                entity.ToTable("Log_Softlandpay");

                entity.Property(e => e.Estado)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdInterno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdTransaccion)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.MedioPago)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MontoBruto)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MontoImpuestos)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MontoTotal)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Origen)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Request).IsUnicode(false);
            });

            modelBuilder.Entity<PagosCabecera>(entity =>
            {
                entity.HasKey(e => e.IdPago);

                entity.ToTable("PagosCabecera");

                entity.Property(e => e.CodAux)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ComprobanteContable)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Correo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaPago).HasColumnType("date");

                entity.Property(e => e.HoraPago)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Rut)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.PagosCabeceras)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK_PagosCabecera_ClientesPortal");

                entity.HasOne(d => d.IdPagoEstadoNavigation)
                    .WithMany(p => p.PagosCabeceras)
                    .HasForeignKey(d => d.IdPagoEstado)
                    .HasConstraintName("FK_PagosCabecera_PagosEstado");
            });

            modelBuilder.Entity<PagosDetalle>(entity =>
            {
                entity.HasKey(e => e.IdPagoDetalle)
                    .HasName("PK_PagoDetalle");

                entity.ToTable("PagosDetalle");

                entity.Property(e => e.Apagar).HasColumnName("APagar");

                entity.Property(e => e.CuentaContableDocumento)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaEmision).HasColumnType("date");

                entity.Property(e => e.FechaVencimiento).HasColumnType("date");

                entity.Property(e => e.TipoDocumento)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdPagoNavigation)
                    .WithMany(p => p.PagosDetalles)
                    .HasForeignKey(d => d.IdPago)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PagosDetalle_PagosCabecera");
            });

            modelBuilder.Entity<PagosEstado>(entity =>
            {
                entity.HasKey(e => e.IdPagosEstado);

                entity.ToTable("PagosEstado");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Parametro>(entity =>
            {
                entity.HasKey(e => e.IdParametro);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Valor)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PasarelaPago>(entity =>
            {
                entity.HasKey(e => e.IdPasarela)
                    .HasName("PK_PasarelasPago");

                entity.ToTable("PasarelaPago");

                entity.Property(e => e.Ambiente)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.AmbienteConsultarPago)
                    .HasMaxLength(2000)
                    .IsUnicode(false);

                entity.Property(e => e.ClaveSoftlandPay)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CodigoComercio).IsUnicode(false);

                entity.Property(e => e.CodigoMedioPagoSoftlandPay)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CuentaContable)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EmpresaSoftlandPay)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Logo)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.MonedaPasarela)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Protocolo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SecretKey).IsUnicode(false);

                entity.Property(e => e.TipoDocumento)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UsuarioSoftlandPay)
                    .HasMaxLength(60)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PasarelaPagoLog>(entity =>
            {
                entity.ToTable("PasarelaPagoLog");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Estado)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.MedioPago)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Monto).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.OrdenCompra)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Tarjeta)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Token)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdPagoNavigation)
                    .WithMany(p => p.PasarelaPagoLogs)
                    .HasForeignKey(d => d.IdPago)
                    .HasConstraintName("FK_PasarelaPagoLog_PagosCabecera");

                entity.HasOne(d => d.IdPasarelaNavigation)
                    .WithMany(p => p.PasarelaPagoLogs)
                    .HasForeignKey(d => d.IdPasarela)
                    .HasConstraintName("FK_PasarelaPagoLog_PasarelaPago");
            });

            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.HasKey(e => e.IdPerfil);

                entity.ToTable("Perfil");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.HasKey(e => e.IdPermiso);

                entity.HasOne(d => d.IdAccesoNavigation)
                    .WithMany(p => p.Permisos)
                    .HasForeignKey(d => d.IdAcceso)
                    .HasConstraintName("FK_Permisos_Acceso");

                entity.HasOne(d => d.IdPerfilNavigation)
                    .WithMany(p => p.Permisos)
                    .HasForeignKey(d => d.IdPerfil)
                    .HasConstraintName("FK_Permisos_Perfil");
            });

            modelBuilder.Entity<PreguntasFrecuente>(entity =>
            {
                entity.HasKey(e => e.IdPregunta);

                entity.Property(e => e.Pregunta)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Respuesta)
                    .HasMaxLength(1000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RegistroEnvioCorreo>(entity =>
            {
                entity.HasKey(e => e.IdRegistro);

                entity.ToTable("RegistroEnvioCorreo");

                entity.Property(e => e.FechaEnvio).HasColumnType("date");

                entity.Property(e => e.HoraEnvio)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoAutomatizacion>(entity =>
            {
                entity.HasKey(e => e.IdTipo);

                entity.ToTable("TipoAutomatizacion");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoCobranza>(entity =>
            {
                entity.HasKey(e => e.IdTipoCobranza);

                entity.ToTable("TipoCobranza");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoEnvio>(entity =>
            {
                entity.HasKey(e => e.IdTipo);

                entity.ToTable("TipoEnvio");

                entity.Property(e => e.IdTipo).HasColumnName("idTipo");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TipoPago>(entity =>
            {
                entity.HasKey(e => e.IdTipoPago);

                entity.ToTable("TipoPago");

                entity.Property(e => e.IdTipoPago).ValueGeneratedNever();

                entity.Property(e => e.CuentaContable)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaMod).HasColumnType("datetime");

                entity.Property(e => e.GeneraDte).HasColumnName("GeneraDTE");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TipoDocumento)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);

                entity.Property(e => e.Apellidos)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FechaCreacion).HasColumnType("datetime");

                entity.Property(e => e.FechaEnvioValidacion).HasColumnType("datetime");

                entity.Property(e => e.Nombres)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdPerfilNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdPerfil)
                    .HasConstraintName("FK_Usuarios_Perfil");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
