# UK Postcode Data Structure

## Overview
* [Type safe](https://en.wikipedia.org/wiki/Type_safety) representation of only those strings that are valid postcodes
* Stack allocated 4 byte struct instead of a .NET string ([26 - 30 byte heap allocation + 4 bytes for a reference](http://www.abstractpath.com/2012/size-of-a-csharp-string/))
* Fast postcode validation and parsing, ignoring spaces and treating all letters case in-sensitively

## Background
Every UK address is associated with a [postcode](https://en.wikipedia.org/wiki/Postcodes_in_the_United_Kingdom). This consists of between 5 and 7 letters and digits in one of these formats:

| Area | District | Sector | Unit | Example  |
|------|----------|--------|------|----------|
| A    | 1        | 1      |  AA  | M1 1AE   |
| A    | 11       | 1      |  AA  | B33 8TH  |
| A    | 1A       | 1      |  AA  | W1A 0AX  |
| AA   | 1        | 1      |  AA  | CR2 6XH  |
| AA   | 11       | 1      |  AA  | DN55 1PT |
| AA   | 1A       | 1      |  AA  | EC1A 1BB |

Where `A` represents an upper case letter A-Z and `1` represents a digit 0-9. A single space is placed between the sector and unit for a total of between 6 and 8 characters.

### Description of Representation
* 1st character of the **Area** is a letter (26 possibilities)
* 2nd character of the **Area** is a letter or missing (26 + 1 = 27 possibilities)
* 1st character of the **District** is a digit (10 possibilities)
* 2nd character of the **District** is a letter, a digit or missing (26 + 10 + 1 = 37 possibilities)
* 1st character of the **Sector** is a digit (10 possibilities)
* 1st character of the **Unit** is a letter (26 possibilities)
* 2nd character of the **Unit** is a letter (26 possibilities)

The number of possible postcodes is
26 · 27 · 10 · 37 · 10 · 26 · 26 = 1,755,842,400 ≤ 2³².

Therefore it is possible to represent a postcode in a 4 byte (32 bit) word by using this scheme.

| Value                    | 0 | 1 | 2 | 3 | … | 9 | 10 | 11 | 12 | 13 | …  | 25 | 26 | … | 36 | 
|--------------------------|---|---|---|---|---|---|--- |----|----|----|----|----|----|---|----|
| Digit                    | 0 | 1 | 2 | 3 | … | 9 |
| Letter                   | A | B | C | D | … | J | K  | L  | M  | N  | …  | Z  |
| Letter or missing        |   | A | B | C | … | K | L  | M  | N  | O  | …  | Y  |  Z |
| Letter, digit or missing |   | 0 | 1 | 2 | … | 8 | 9  | A  | B  | C  | …  | M  |  N | … |  Z |

### Parsing Algorithm
``` C#
public static bool TryParse(string s, out Postcode postcode)
```
The algorithm can be understood as a special case of [multi-dimensional](https://en.wikipedia.org/wiki/Array_data_structure#Multidimensional_arrays) array indexing:
* Imagine each valid postcode is placed in a 7 dimensional array with the indexes for each dimension given by the scheme above
* This array might be represented in memory by a single dimensional array
* By calculating the index for a given postcode in this single dimensional array we have an integer that uniquely specifies any valid postcode
* It is not necessary to store the hypothetical array since the element the index represents can be easily computed using the reverse calculation
