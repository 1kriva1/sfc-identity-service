import { Component, Input } from '@angular/core';
import { environment } from '@environments/environment';

@Component({
  selector: 'sfc-logo',
  templateUrl: './logo.component.html',
  styleUrls: ['./logo.component.scss']
})
export class LogoComponent {

  environment = environment;

  @Input()
  showTitle: boolean = true;
}
