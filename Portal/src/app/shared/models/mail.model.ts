import { MailTipo } from '../enums/MailTipo';

export class Mail {
    mailTipo?: MailTipo;
    nombreDestinatario?: string;
    to?: string;
    cC?: string;
    telefono?: string;
    subject?: string;
    message?: string;
    linkUrl?: string;
}