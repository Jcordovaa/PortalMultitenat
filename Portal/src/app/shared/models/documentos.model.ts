export class Documento {
   documento?: string;
   nro?: number;
   fechaEmision?: Date;
   fechaVcto?: Date;
   debe?: number;
   haber?: number;
   saldo?: number;
   detalle?: string;
   estado?: string;
   pago?: string;
   tipoDoc?: string;
   razonSocial?: string;
   comprobanteContable?: string;
   cuentaContable?: string;
   aPagar?: number;
   montoBase?: number;
   saldoBase?: number;
   codigoMoneda?: string;
   equivalenciaMoneda?: number;
   montoOriginalBase?: number;
   codAux?: string;
   desMon?: string;
   
   constructor() {
       this.documento = '',
       this.nro = 0,
       this.fechaEmision = new Date(),
       this.fechaVcto = new Date(),
       this.debe = 0,
       this.haber = 0,
       this.saldo = 0,
       this.detalle = '',
       this.estado = '',
       this.pago = '',
       this.pago = '',
       this.tipoDoc = '',
       this.razonSocial = '',
       this.comprobanteContable = '',
       this.cuentaContable = '',
       this.aPagar = 0;
       this.montoBase = 0;
       this.saldoBase = 0;
       this.codigoMoneda = '';
       this.equivalenciaMoneda = 0;
       this.montoOriginalBase = 0;
       this.codAux = '';
       this.desMon = '';
   }
}
