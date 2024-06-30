import { AfterViewChecked, ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { environment } from '@environments/environment';
import { ButtonType, MediaLimits, WINDOW } from 'ngx-sfc-common';
import { NotFoundPageConstants } from './not-found.page.constants';
import { buildTitle } from '../../utils';

@Component({
    templateUrl: './not-found.page.component.html',
    styleUrls: ['./not-found.page.component.scss']
})
export class NotFoundPageComponent implements OnInit, AfterViewChecked {

    environment = environment;

    ButtonType = ButtonType;

    BUTTON_BACK_TEXT = $localize`:@@core.page.not-found.button-back:BACK THE GAME`;

    public get size(): number {
        return NotFoundPageConstants.DEFAULT_SIZE * this.sizeFactor;
    }

    private get sizeFactor() {
        if (this.window.outerWidth <= MediaLimits.Tablet)
            return NotFoundPageConstants.TABLET_AND_LESS_SIZE_FACTOR * this.window.outerWidth / MediaLimits.LaptopLarge;
        else
            return NotFoundPageConstants.DEFAULT_SIZE_FACTOR * this.window.outerWidth / MediaLimits.LaptopLarge;
    }

    constructor(
        private changeDetector: ChangeDetectorRef,
        @Inject(WINDOW) private window: Window,
        private titleService: Title) { }

    ngOnInit(): void {
        this.titleService.setTitle(buildTitle($localize`:@@core.page.not-found.page-title:Not found`));
    }

    ngAfterViewChecked(): void {
        this.changeDetector.detectChanges();
    }
}
