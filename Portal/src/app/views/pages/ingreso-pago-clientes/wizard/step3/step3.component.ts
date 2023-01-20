import { Component, OnInit, Input } from '@angular/core';
import { WizardData } from '../../../../../shared/models/wizarddata.model';

@Component({
  selector: 'app-step3',
  templateUrl: './step3.component.html'
})
export class Step3Component implements OnInit {

  @Input() resume: WizardData;

  constructor() { }

  ngOnInit(): void {
  }

}
