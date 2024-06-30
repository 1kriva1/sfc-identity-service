import { empty } from "ngx-sfc-common";

export interface IIdentityNavigationModel {
    link: string;
    params: { [id: string]: string | empty; }
}