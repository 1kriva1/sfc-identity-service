import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { environment } from '@environments/environment';
import { faCircleXmark } from '@fortawesome/free-regular-svg-icons';
import { ButtonType } from 'ngx-sfc-common';
import { buildTitle } from '../../utils';

@Component({
    templateUrl: './error.page.component.html',
    styleUrls: ['./error.page.component.scss']
})
export class ErrorPageComponent implements OnInit {

    faCircleXmark = faCircleXmark;

    ButtonType = ButtonType;

    environment = environment;

    public BUTTON_BACK = $localize`:@@core.page.error.button-back:Back to SFC`;

    constructor(private titleService: Title) { }

    ngOnInit(): void {
        this.titleService.setTitle(buildTitle($localize`:@@core.page.error.page-title:Error`));
    }
}