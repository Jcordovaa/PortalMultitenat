import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';


import { CalendarRoutingModule } from './calendar-routing.module';
import { CalendarComponent } from './calendar/calendar.component';
import { CalendarFormDialogComponent } from './calendar-form-dialog/calendar-form-dialog.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ReactiveFormsModule } from '@angular/forms';
import { ColorPickerModule } from 'ngx-color-picker';


@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NgbModule,
    ColorPickerModule,
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory
    }),
    CalendarRoutingModule
  ],
  declarations: [CalendarComponent, CalendarFormDialogComponent],
  entryComponents: [CalendarFormDialogComponent]
})
export class CalendarAppModule { }
