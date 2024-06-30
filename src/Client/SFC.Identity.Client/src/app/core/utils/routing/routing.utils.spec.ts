import { buildPath, buildTitle } from "./routing.utils";

describe('Core.Utils: Routing', () => {
    fit('Should build path', () => {
        expect(buildPath('test')).toEqual('/test');
    });

    fit('Should build title', () => {
        expect(buildTitle('test')).toEqual('SFC - test');
    });
});
