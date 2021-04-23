/**
 * logic_seededname.ts
 * 
 * Code to create a random name from a seed
 */

export class Logic_SeededName {

    public static GetNameFromString(_nameSeed: string): string {
        let newName: string = '';

        let hashes: number[] = this.GetHashes(_nameSeed);

        //Use hashes to make names

        //console.log('snc hashes ' + hashes[0] + ' ' + hashes[1]);
        newName = this.nameStore[0][hashes[0] % this.nameStore[0].length] + ' ' + this.nameStore[1][hashes[1] % this.nameStore[1].length];

        return newName;
    }

    protected static GetHashes(hashString: string): number [] {
        
        //Get the hash of the string forwards

        let idUTF: number[] = [];
        let hashes: number[] = [];

        for(let i: number = 0; i < hashString.length; i++) {
            let code: number = hashString.charCodeAt(i);
            if (code < 0x80) {
                idUTF.push(code);
            } else if (code < 0x800) {
                idUTF.push(0xc0 | (code >> 6));
                idUTF.push(0xc0 | (code & 0x3f));
            }
            else if (code < 0xd800 || code >= 0xe000) {
                idUTF.push(0xe0 | (code >> 12));
                idUTF.push(0x80 | ((code >> 6) & 0x3f));
                idUTF.push(0x80 | (code & 0x3f));
            }
            else {
                //Double byte input
                let code2: number = hashString.charCodeAt(i++);
                let fullCode: number = 0x10000 + ((code & 0x3ff) << 10) | ((code2 & 0x3ff));
                idUTF.push(0xf0 | (fullCode >> 18));
                idUTF.push(0x80 | ((code >> 12) & 0x3f));
                idUTF.push(0x80 | ((code >> 6) & 0x3f));
                idUTF.push(0x80 | (code & 0x3f));
            }
        }

        //Make hashes

        hashes[0] = 0;
        hashes[1] = 0;

        for(let i: number = 0; i < idUTF.length; i++) {
            hashes[0] = Math.abs(((hashes[0] * 31) ^ idUTF[i]));
            hashes[1] = Math.abs(((hashes[1] * 31) ^ idUTF[idUTF.length - 1 - i]));

        }
        
        return hashes;        
    }

    protected static nameStore: string[][] = [
        //First names
        [
            'fred',
            'bob',
            'wilma'
        ],
        //Last names
        [
            'rubble',
            'flintstone'
        ]
    ]


}

