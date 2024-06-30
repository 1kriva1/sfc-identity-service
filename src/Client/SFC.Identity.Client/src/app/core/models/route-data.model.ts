import { ILayoutModel } from "./layout.model";
import { IThemeModel } from "./theme.model";

export interface IRouteDataModel {
  layout: ILayoutModel;
  theme?: IThemeModel;
}
