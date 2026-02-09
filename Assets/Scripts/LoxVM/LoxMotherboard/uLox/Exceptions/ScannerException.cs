using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ULox
{
    public class ScannerException : UloxException
    {
        public ScannerException(string msg, TokenType tokenType, int line, int character, string location)
            : base($"{msg} got {tokenType} in {location} at {line}:{character}") { }
    }
}
