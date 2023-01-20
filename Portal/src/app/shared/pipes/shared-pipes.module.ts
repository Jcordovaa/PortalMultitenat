import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExcerptPipe } from './excerpt.pipe';
import { GetValueByKeyPipe } from './get-value-by-key.pipe';
import { RelativeTimePipe } from './relative-time.pipe';
import { SinImagenPipe } from './sinimagen.pipe';
import { MontoPipe } from './monto.pipe';
import { LimitToPipe } from './limitto.pipe';
import { SinTextoPipe } from './sintexto.pipe';

const pipes = [
  ExcerptPipe,
  GetValueByKeyPipe,
  RelativeTimePipe,
  SinImagenPipe,
  MontoPipe,
  LimitToPipe,
  SinTextoPipe
];

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: pipes,
  exports: pipes
})
export class SharedPipesModule { }
