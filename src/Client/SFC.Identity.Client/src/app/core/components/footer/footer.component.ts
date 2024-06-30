import { Component } from '@angular/core';

@Component({
  selector: 'sfc-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent {

  public get year(): number {
    return new Date().getFullYear();
  }
}
