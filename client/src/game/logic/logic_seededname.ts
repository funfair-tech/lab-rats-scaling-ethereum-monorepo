/**
 * logic_seededname.ts
 * 
 * Code to create a name from a seed
 */

export class Logic_SeededName {

    public static GetNameFromString(_nameSeed: string): string {        
        let newName: string = '';

        //For now, supply truncated address as name

        newName = _nameSeed.substr(0, 4) + '...' + _nameSeed.substr(_nameSeed.length - 4, 4);
        console.log('for nameseed ' + _nameSeed + ' we have ' + newName);
        return newName;
    }

}

