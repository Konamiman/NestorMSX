// $ANTLR 3.0.1 ECalc.g 2007-11-14 09:02:20

using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;



public class ECalcLexer : Lexer 
{
    public const int INTEGER = 22;
    public const int LT = 11;
    public const int MOD = 19;
    public const int DATETIME = 24;
    public const int LTEQ = 12;
    public const int NEGATE = 5;
    public const int NOTEQUALS = 10;
    public const int PARAM = 4;
    public const int FLOAT = 23;
    public const int EQUALS = 9;
    public const int NOT = 21;
    public const int GTEQ = 14;
    public const int MINUS = 16;
    public const int MULT = 17;
    public const int AND = 8;
    public const int Tokens = 36;
    public const int EOF = -1;
    public const int HexDigit = 29;
    public const int BOOLEAN = 25;
    public const int WS = 30;
    public const int POW = 20;
    public const int UnicodeEscape = 28;
    public const int OR = 7;
    public const int GT = 13;
    public const int IDENT = 26;
    public const int PLUS = 15;
    public const int DIV = 18;
    public const int T34 = 34;
    public const int T33 = 33;
    public const int T35 = 35;
    public const int EscapeSequence = 27;
    public const int T32 = 32;
    public const int STRING = 6;
    public const int T31 = 31;

    public ECalcLexer() 
    {
		InitializeCyclicDFAs();
    }
    public ECalcLexer(ICharStream input) 
		: base(input)
	{
		InitializeCyclicDFAs();
    }
    
    override public string GrammarFileName
    {
    	get { return "ECalc.g";} 
    }

    // $ANTLR start T31 
    public void mT31() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = T31;
            // ECalc.g:7:5: ( '(' )
            // ECalc.g:7:7: '('
            {
            	Match('('); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end T31

    // $ANTLR start T32 
    public void mT32() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = T32;
            // ECalc.g:8:5: ( ')' )
            // ECalc.g:8:7: ')'
            {
            	Match(')'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end T32

    // $ANTLR start T33 
    public void mT33() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = T33;
            // ECalc.g:9:5: ( '[' )
            // ECalc.g:9:7: '['
            {
            	Match('['); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end T33

    // $ANTLR start T34 
    public void mT34() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = T34;
            // ECalc.g:10:5: ( ']' )
            // ECalc.g:10:7: ']'
            {
            	Match(']'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end T34

    // $ANTLR start T35 
    public void mT35() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = T35;
            // ECalc.g:11:5: ( ',' )
            // ECalc.g:11:7: ','
            {
            	Match(','); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end T35

    // $ANTLR start OR 
    public void mOR() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = OR;
            // ECalc.g:44:5: ( '||' | 'or' )
            int alt1 = 2;
            int LA1_0 = input.LA(1);
            
            if ( (LA1_0 == '|') )
            {
                alt1 = 1;
            }
            else if ( (LA1_0 == 'o') )
            {
                alt1 = 2;
            }
            else 
            {
                NoViableAltException nvae_d1s0 =
                    new NoViableAltException("44:1: OR : ( '||' | 'or' );", 1, 0, input);
            
                throw nvae_d1s0;
            }
            switch (alt1) 
            {
                case 1 :
                    // ECalc.g:44:8: '||'
                    {
                    	Match("||"); 

                    
                    }
                    break;
                case 2 :
                    // ECalc.g:44:15: 'or'
                    {
                    	Match("or"); 

                    
                    }
                    break;
            
            }
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end OR

    // $ANTLR start AND 
    public void mAND() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = AND;
            // ECalc.g:50:6: ( '&&' | 'and' )
            int alt2 = 2;
            int LA2_0 = input.LA(1);
            
            if ( (LA2_0 == '&') )
            {
                alt2 = 1;
            }
            else if ( (LA2_0 == 'a') )
            {
                alt2 = 2;
            }
            else 
            {
                NoViableAltException nvae_d2s0 =
                    new NoViableAltException("50:1: AND : ( '&&' | 'and' );", 2, 0, input);
            
                throw nvae_d2s0;
            }
            switch (alt2) 
            {
                case 1 :
                    // ECalc.g:50:9: '&&'
                    {
                    	Match("&&"); 

                    
                    }
                    break;
                case 2 :
                    // ECalc.g:50:16: 'and'
                    {
                    	Match("and"); 

                    
                    }
                    break;
            
            }
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end AND

    // $ANTLR start EQUALS 
    public void mEQUALS() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = EQUALS;
            // ECalc.g:57:2: ( '=' | '==' )
            int alt3 = 2;
            int LA3_0 = input.LA(1);
            
            if ( (LA3_0 == '=') )
            {
                int LA3_1 = input.LA(2);
                
                if ( (LA3_1 == '=') )
                {
                    alt3 = 2;
                }
                else 
                {
                    alt3 = 1;}
            }
            else 
            {
                NoViableAltException nvae_d3s0 =
                    new NoViableAltException("56:1: EQUALS : ( '=' | '==' );", 3, 0, input);
            
                throw nvae_d3s0;
            }
            switch (alt3) 
            {
                case 1 :
                    // ECalc.g:57:4: '='
                    {
                    	Match('='); 
                    
                    }
                    break;
                case 2 :
                    // ECalc.g:57:10: '=='
                    {
                    	Match("=="); 

                    
                    }
                    break;
            
            }
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end EQUALS

    // $ANTLR start NOTEQUALS 
    public void mNOTEQUALS() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = NOTEQUALS;
            // ECalc.g:59:2: ( '!=' | '<>' )
            int alt4 = 2;
            int LA4_0 = input.LA(1);
            
            if ( (LA4_0 == '!') )
            {
                alt4 = 1;
            }
            else if ( (LA4_0 == '<') )
            {
                alt4 = 2;
            }
            else 
            {
                NoViableAltException nvae_d4s0 =
                    new NoViableAltException("58:1: NOTEQUALS : ( '!=' | '<>' );", 4, 0, input);
            
                throw nvae_d4s0;
            }
            switch (alt4) 
            {
                case 1 :
                    // ECalc.g:59:4: '!='
                    {
                    	Match("!="); 

                    
                    }
                    break;
                case 2 :
                    // ECalc.g:59:11: '<>'
                    {
                    	Match("<>"); 

                    
                    }
                    break;
            
            }
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end NOTEQUALS

    // $ANTLR start LT 
    public void mLT() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = LT;
            // ECalc.g:65:4: ( '<' )
            // ECalc.g:65:6: '<'
            {
            	Match('<'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end LT

    // $ANTLR start LTEQ 
    public void mLTEQ() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = LTEQ;
            // ECalc.g:66:6: ( '<=' )
            // ECalc.g:66:8: '<='
            {
            	Match("<="); 

            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end LTEQ

    // $ANTLR start GT 
    public void mGT() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = GT;
            // ECalc.g:67:4: ( '>' )
            // ECalc.g:67:6: '>'
            {
            	Match('>'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end GT

    // $ANTLR start GTEQ 
    public void mGTEQ() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = GTEQ;
            // ECalc.g:68:6: ( '>=' )
            // ECalc.g:68:8: '>='
            {
            	Match(">="); 

            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end GTEQ

    // $ANTLR start PLUS 
    public void mPLUS() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = PLUS;
            // ECalc.g:74:6: ( '+' )
            // ECalc.g:74:8: '+'
            {
            	Match('+'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end PLUS

    // $ANTLR start MINUS 
    public void mMINUS() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = MINUS;
            // ECalc.g:75:7: ( '-' )
            // ECalc.g:75:9: '-'
            {
            	Match('-'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end MINUS

    // $ANTLR start MULT 
    public void mMULT() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = MULT;
            // ECalc.g:81:6: ( '*' )
            // ECalc.g:81:8: '*'
            {
            	Match('*'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end MULT

    // $ANTLR start DIV 
    public void mDIV() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = DIV;
            // ECalc.g:82:5: ( '/' )
            // ECalc.g:82:7: '/'
            {
            	Match('/'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end DIV

    // $ANTLR start MOD 
    public void mMOD() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = MOD;
            // ECalc.g:83:5: ( '%' )
            // ECalc.g:83:7: '%'
            {
            	Match('%'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end MOD

    // $ANTLR start POW 
    public void mPOW() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = POW;
            // ECalc.g:89:5: ( '^' )
            // ECalc.g:89:7: '^'
            {
            	Match('^'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end POW

    // $ANTLR start NOT 
    public void mNOT() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = NOT;
            // ECalc.g:97:5: ( '!' | 'not' )
            int alt5 = 2;
            int LA5_0 = input.LA(1);
            
            if ( (LA5_0 == '!') )
            {
                alt5 = 1;
            }
            else if ( (LA5_0 == 'n') )
            {
                alt5 = 2;
            }
            else 
            {
                NoViableAltException nvae_d5s0 =
                    new NoViableAltException("97:1: NOT : ( '!' | 'not' );", 5, 0, input);
            
                throw nvae_d5s0;
            }
            switch (alt5) 
            {
                case 1 :
                    // ECalc.g:97:7: '!'
                    {
                    	Match('!'); 
                    
                    }
                    break;
                case 2 :
                    // ECalc.g:97:13: 'not'
                    {
                    	Match("not"); 

                    
                    }
                    break;
            
            }
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end NOT

    // $ANTLR start STRING 
    public void mSTRING() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = STRING;
            // ECalc.g:119:6: ( '\\'' ( EscapeSequence | ( options {greedy=false; } : ~ ( '\\u0000' .. '\\u001f' | '\\\\' | '\\'' ) ) )* '\\'' )
            // ECalc.g:119:10: '\\'' ( EscapeSequence | ( options {greedy=false; } : ~ ( '\\u0000' .. '\\u001f' | '\\\\' | '\\'' ) ) )* '\\''
            {
            	Match('\''); 
            	// ECalc.g:119:15: ( EscapeSequence | ( options {greedy=false; } : ~ ( '\\u0000' .. '\\u001f' | '\\\\' | '\\'' ) ) )*
            	do 
            	{
            	    int alt6 = 3;
            	    int LA6_0 = input.LA(1);
            	    
            	    if ( (LA6_0 == '\\') )
            	    {
            	        alt6 = 1;
            	    }
            	    else if ( ((LA6_0 >= ' ' && LA6_0 <= '&') || (LA6_0 >= '(' && LA6_0 <= '[') || (LA6_0 >= ']' && LA6_0 <= '\uFFFE')) )
            	    {
            	        alt6 = 2;
            	    }
            	    
            	
            	    switch (alt6) 
            		{
            			case 1 :
            			    // ECalc.g:119:17: EscapeSequence
            			    {
            			    	mEscapeSequence(); 
            			    
            			    }
            			    break;
            			case 2 :
            			    // ECalc.g:119:34: ( options {greedy=false; } : ~ ( '\\u0000' .. '\\u001f' | '\\\\' | '\\'' ) )
            			    {
            			    	// ECalc.g:119:34: ( options {greedy=false; } : ~ ( '\\u0000' .. '\\u001f' | '\\\\' | '\\'' ) )
            			    	// ECalc.g:119:61: ~ ( '\\u0000' .. '\\u001f' | '\\\\' | '\\'' )
            			    	{
            			    		if ( (input.LA(1) >= ' ' && input.LA(1) <= '&') || (input.LA(1) >= '(' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\uFFFE') ) 
            			    		{
            			    		    input.Consume();
            			    		
            			    		}
            			    		else 
            			    		{
            			    		    MismatchedSetException mse =
            			    		        new MismatchedSetException(null,input);
            			    		    Recover(mse);    throw mse;
            			    		}

            			    	
            			    	}

            			    
            			    }
            			    break;
            	
            			default:
            			    goto loop6;
            	    }
            	} while (true);
            	
            	loop6:
            		;	// Stops C# compiler whinging that label 'loop6' has no statements

            	Match('\''); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end STRING

    // $ANTLR start INTEGER 
    public void mINTEGER() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = INTEGER;
            // ECalc.g:123:2: ( ( '0' .. '9' )+ )
            // ECalc.g:123:4: ( '0' .. '9' )+
            {
            	// ECalc.g:123:4: ( '0' .. '9' )+
            	int cnt7 = 0;
            	do 
            	{
            	    int alt7 = 2;
            	    int LA7_0 = input.LA(1);
            	    
            	    if ( ((LA7_0 >= '0' && LA7_0 <= '9')) )
            	    {
            	        alt7 = 1;
            	    }
            	    
            	
            	    switch (alt7) 
            		{
            			case 1 :
            			    // ECalc.g:123:5: '0' .. '9'
            			    {
            			    	MatchRange('0','9'); 
            			    
            			    }
            			    break;
            	
            			default:
            			    if ( cnt7 >= 1 ) goto loop7;
            		            EarlyExitException eee =
            		                new EarlyExitException(7, input);
            		            throw eee;
            	    }
            	    cnt7++;
            	} while (true);
            	
            	loop7:
            		;	// Stops C# compiler whinging that label 'loop7' has no statements

            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end INTEGER

    // $ANTLR start FLOAT 
    public void mFLOAT() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = FLOAT;
            // ECalc.g:127:2: ( ( '0' .. '9' )* '.' ( '0' .. '9' )+ )
            // ECalc.g:127:4: ( '0' .. '9' )* '.' ( '0' .. '9' )+
            {
            	// ECalc.g:127:4: ( '0' .. '9' )*
            	do 
            	{
            	    int alt8 = 2;
            	    int LA8_0 = input.LA(1);
            	    
            	    if ( ((LA8_0 >= '0' && LA8_0 <= '9')) )
            	    {
            	        alt8 = 1;
            	    }
            	    
            	
            	    switch (alt8) 
            		{
            			case 1 :
            			    // ECalc.g:127:5: '0' .. '9'
            			    {
            			    	MatchRange('0','9'); 
            			    
            			    }
            			    break;
            	
            			default:
            			    goto loop8;
            	    }
            	} while (true);
            	
            	loop8:
            		;	// Stops C# compiler whinging that label 'loop8' has no statements

            	Match('.'); 
            	// ECalc.g:127:20: ( '0' .. '9' )+
            	int cnt9 = 0;
            	do 
            	{
            	    int alt9 = 2;
            	    int LA9_0 = input.LA(1);
            	    
            	    if ( ((LA9_0 >= '0' && LA9_0 <= '9')) )
            	    {
            	        alt9 = 1;
            	    }
            	    
            	
            	    switch (alt9) 
            		{
            			case 1 :
            			    // ECalc.g:127:21: '0' .. '9'
            			    {
            			    	MatchRange('0','9'); 
            			    
            			    }
            			    break;
            	
            			default:
            			    if ( cnt9 >= 1 ) goto loop9;
            		            EarlyExitException eee =
            		                new EarlyExitException(9, input);
            		            throw eee;
            	    }
            	    cnt9++;
            	} while (true);
            	
            	loop9:
            		;	// Stops C# compiler whinging that label 'loop9' has no statements

            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end FLOAT

    // $ANTLR start DATETIME 
    public void mDATETIME() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = DATETIME;
            // ECalc.g:131:3: ( '#' (~ '#' )* '#' )
            // ECalc.g:131:5: '#' (~ '#' )* '#'
            {
            	Match('#'); 
            	// ECalc.g:131:9: (~ '#' )*
            	do 
            	{
            	    int alt10 = 2;
            	    int LA10_0 = input.LA(1);
            	    
            	    if ( ((LA10_0 >= '\u0000' && LA10_0 <= '\"') || (LA10_0 >= '$' && LA10_0 <= '\uFFFE')) )
            	    {
            	        alt10 = 1;
            	    }
            	    
            	
            	    switch (alt10) 
            		{
            			case 1 :
            			    // ECalc.g:131:10: ~ '#'
            			    {
            			    	if ( (input.LA(1) >= '\u0000' && input.LA(1) <= '\"') || (input.LA(1) >= '$' && input.LA(1) <= '\uFFFE') ) 
            			    	{
            			    	    input.Consume();
            			    	
            			    	}
            			    	else 
            			    	{
            			    	    MismatchedSetException mse =
            			    	        new MismatchedSetException(null,input);
            			    	    Recover(mse);    throw mse;
            			    	}

            			    
            			    }
            			    break;
            	
            			default:
            			    goto loop10;
            	    }
            	} while (true);
            	
            	loop10:
            		;	// Stops C# compiler whinging that label 'loop10' has no statements

            	Match('#'); 
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end DATETIME

    // $ANTLR start BOOLEAN 
    public void mBOOLEAN() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = BOOLEAN;
            // ECalc.g:135:2: ( 'true' | 'false' )
            int alt11 = 2;
            int LA11_0 = input.LA(1);
            
            if ( (LA11_0 == 't') )
            {
                alt11 = 1;
            }
            else if ( (LA11_0 == 'f') )
            {
                alt11 = 2;
            }
            else 
            {
                NoViableAltException nvae_d11s0 =
                    new NoViableAltException("134:1: BOOLEAN : ( 'true' | 'false' );", 11, 0, input);
            
                throw nvae_d11s0;
            }
            switch (alt11) 
            {
                case 1 :
                    // ECalc.g:135:4: 'true'
                    {
                    	Match("true"); 

                    
                    }
                    break;
                case 2 :
                    // ECalc.g:136:4: 'false'
                    {
                    	Match("false"); 

                    
                    }
                    break;
            
            }
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end BOOLEAN

    // $ANTLR start IDENT 
    public void mIDENT() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = IDENT;
            // ECalc.g:145:2: ( ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )* )
            // ECalc.g:145:4: ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
            {
            	if ( (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
            	{
            	    input.Consume();
            	
            	}
            	else 
            	{
            	    MismatchedSetException mse =
            	        new MismatchedSetException(null,input);
            	    Recover(mse);    throw mse;
            	}

            	// ECalc.g:145:32: ( 'a' .. 'z' | 'A' .. 'Z' | '_' | '0' .. '9' )*
            	do 
            	{
            	    int alt12 = 2;
            	    int LA12_0 = input.LA(1);
            	    
            	    if ( ((LA12_0 >= '0' && LA12_0 <= '9') || (LA12_0 >= 'A' && LA12_0 <= 'Z') || LA12_0 == '_' || LA12_0 == '.' || (LA12_0 >= 'a' && LA12_0 <= 'z')) )
            	    {
            	        alt12 = 1;
            	    }
            	    
            	
            	    switch (alt12) 
            		{
            			case 1 :
            			    // ECalc.g:
            			    {
            			    	if ( (input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || input.LA(1) == '.' || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
            			    	{
            			    	    input.Consume();
            			    	
            			    	}
            			    	else 
            			    	{
            			    	    MismatchedSetException mse =
            			    	        new MismatchedSetException(null,input);
            			    	    Recover(mse);    throw mse;
            			    	}

            			    
            			    }
            			    break;
            	
            			default:
            			    goto loop12;
            	    }
            	} while (true);
            	
            	loop12:
            		;	// Stops C# compiler whinging that label 'loop12' has no statements

            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end IDENT

    // $ANTLR start EscapeSequence 
    public void mEscapeSequence() // throws RecognitionException [2]
    {
        try 
    	{
            // ECalc.g:149:2: ( '\\\\' ( 'n' | 'r' | 't' | '\\'' | '\\\\' | UnicodeEscape ) )
            // ECalc.g:149:4: '\\\\' ( 'n' | 'r' | 't' | '\\'' | '\\\\' | UnicodeEscape )
            {
            	Match('\\'); 
            	// ECalc.g:150:4: ( 'n' | 'r' | 't' | '\\'' | '\\\\' | UnicodeEscape )
            	int alt13 = 6;
            	switch ( input.LA(1) ) 
            	{
            	case 'n':
            		{
            	    alt13 = 1;
            	    }
            	    break;
            	case 'r':
            		{
            	    alt13 = 2;
            	    }
            	    break;
            	case 't':
            		{
            	    alt13 = 3;
            	    }
            	    break;
                case '\'':
            		{
            	    alt13 = 4;
            	    }
            	    break;
            	case '\\':
            		{
            	    alt13 = 5;
            	    }
            	    break;
            	case 'u':
            		{
            	    alt13 = 6;
            	    }
            	    break;
            		default:
            		    NoViableAltException nvae_d13s0 =
            		        new NoViableAltException("150:4: ( 'n' | 'r' | 't' | '\\'' | '\\\\' | UnicodeEscape )", 13, 0, input);
            	
            		    throw nvae_d13s0;
            	}
            	
            	switch (alt13) 
            	{
            	    case 1 :
            	        // ECalc.g:151:5: 'n'
            	        {
            	        	Match('n'); 
            	        
            	        }
            	        break;
            	    case 2 :
            	        // ECalc.g:152:4: 'r'
            	        {
            	        	Match('r'); 
            	        
            	        }
            	        break;
            	    case 3 :
            	        // ECalc.g:153:4: 't'
            	        {
            	        	Match('t'); 
            	        
            	        }
            	        break;
            	    case 4 :
            	        // ECalc.g:154:4: '\\''
            	        {
            	        	Match('\''); 
            	        }
            	        break;
            	    case 5 :
            	        // ECalc.g:155:4: '\\\\'
            	        {
            	        	Match('\\'); 
            	        
            	        }
            	        break;
            	    case 6 :
            	        // ECalc.g:156:4: UnicodeEscape
            	        {
            	        	mUnicodeEscape(); 
            	        
            	        }
            	        break;
            	
            	}

            
            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end EscapeSequence

    // $ANTLR start UnicodeEscape 
    public void mUnicodeEscape() // throws RecognitionException [2]
    {
        try 
    	{
            // ECalc.g:161:6: ( 'u' HexDigit HexDigit HexDigit HexDigit )
            // ECalc.g:161:12: 'u' HexDigit HexDigit HexDigit HexDigit
            {
            	Match('u'); 
            	mHexDigit(); 
            	mHexDigit(); 
            	mHexDigit(); 
            	mHexDigit(); 
            
            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end UnicodeEscape

    // $ANTLR start HexDigit 
    public void mHexDigit() // throws RecognitionException [2]
    {
        try 
    	{
            // ECalc.g:165:2: ( ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' ) )
            // ECalc.g:165:5: ( '0' .. '9' | 'a' .. 'f' | 'A' .. 'F' )
            {
            	if ( (input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'A' && input.LA(1) <= 'F') || (input.LA(1) >= 'a' && input.LA(1) <= 'f') ) 
            	{
            	    input.Consume();
            	
            	}
            	else 
            	{
            	    MismatchedSetException mse =
            	        new MismatchedSetException(null,input);
            	    Recover(mse);    throw mse;
            	}

            
            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end HexDigit

    // $ANTLR start WS 
    public void mWS() // throws RecognitionException [2]
    {
        try 
    	{
            int _type = WS;
            // ECalc.g:169:2: ( ( ' ' | '\\r' | '\\t' | '\\u000C' | '\\n' ) )
            // ECalc.g:169:5: ( ' ' | '\\r' | '\\t' | '\\u000C' | '\\n' )
            {
            	if ( (input.LA(1) >= '\t' && input.LA(1) <= '\n') || (input.LA(1) >= '\f' && input.LA(1) <= '\r') || input.LA(1) == ' ' ) 
            	{
            	    input.Consume();
            	
            	}
            	else 
            	{
            	    MismatchedSetException mse =
            	        new MismatchedSetException(null,input);
            	    Recover(mse);    throw mse;
            	}

            	channel=HIDDEN;
            
            }
    
            this.type = _type;
        }
        finally 
    	{
        }
    }
    // $ANTLR end WS

    override public void mTokens() // throws RecognitionException 
    {
        // ECalc.g:1:8: ( T31 | T32 | T33 | T34 | T35 | OR | AND | EQUALS | NOTEQUALS | LT | LTEQ | GT | GTEQ | PLUS | MINUS | MULT | DIV | MOD | POW | NOT | STRING | INTEGER | FLOAT | DATETIME | BOOLEAN | IDENT | WS )
        int alt14 = 27;
        alt14 = dfa14.Predict(input);
        switch (alt14) 
        {
            case 1 :
                // ECalc.g:1:10: T31
                {
                	mT31(); 
                
                }
                break;
            case 2 :
                // ECalc.g:1:14: T32
                {
                	mT32(); 
                
                }
                break;
            case 3 :
                // ECalc.g:1:18: T33
                {
                	mT33(); 
                
                }
                break;
            case 4 :
                // ECalc.g:1:22: T34
                {
                	mT34(); 
                
                }
                break;
            case 5 :
                // ECalc.g:1:26: T35
                {
                	mT35(); 
                
                }
                break;
            case 6 :
                // ECalc.g:1:30: OR
                {
                	mOR(); 
                
                }
                break;
            case 7 :
                // ECalc.g:1:33: AND
                {
                	mAND(); 
                
                }
                break;
            case 8 :
                // ECalc.g:1:37: EQUALS
                {
                	mEQUALS(); 
                
                }
                break;
            case 9 :
                // ECalc.g:1:44: NOTEQUALS
                {
                	mNOTEQUALS(); 
                
                }
                break;
            case 10 :
                // ECalc.g:1:54: LT
                {
                	mLT(); 
                
                }
                break;
            case 11 :
                // ECalc.g:1:57: LTEQ
                {
                	mLTEQ(); 
                
                }
                break;
            case 12 :
                // ECalc.g:1:62: GT
                {
                	mGT(); 
                
                }
                break;
            case 13 :
                // ECalc.g:1:65: GTEQ
                {
                	mGTEQ(); 
                
                }
                break;
            case 14 :
                // ECalc.g:1:70: PLUS
                {
                	mPLUS(); 
                
                }
                break;
            case 15 :
                // ECalc.g:1:75: MINUS
                {
                	mMINUS(); 
                
                }
                break;
            case 16 :
                // ECalc.g:1:81: MULT
                {
                	mMULT(); 
                
                }
                break;
            case 17 :
                // ECalc.g:1:86: DIV
                {
                	mDIV(); 
                
                }
                break;
            case 18 :
                // ECalc.g:1:90: MOD
                {
                	mMOD(); 
                
                }
                break;
            case 19 :
                // ECalc.g:1:94: POW
                {
                	mPOW(); 
                
                }
                break;
            case 20 :
                // ECalc.g:1:98: NOT
                {
                	mNOT(); 
                
                }
                break;
            case 21 :
                // ECalc.g:1:102: STRING
                {
                	mSTRING(); 
                
                }
                break;
            case 22 :
                // ECalc.g:1:109: INTEGER
                {
                	mINTEGER(); 
                
                }
                break;
            case 23 :
                // ECalc.g:1:117: FLOAT
                {
                	mFLOAT(); 
                
                }
                break;
            case 24 :
                // ECalc.g:1:123: DATETIME
                {
                	mDATETIME(); 
                
                }
                break;
            case 25 :
                // ECalc.g:1:132: BOOLEAN
                {
                	mBOOLEAN(); 
                
                }
                break;
            case 26 :
                // ECalc.g:1:140: IDENT
                {
                	mIDENT(); 
                
                }
                break;
            case 27 :
                // ECalc.g:1:146: WS
                {
                	mWS(); 
                
                }
                break;
        
        }
    
    }


    protected DFA14 dfa14;
	private void InitializeCyclicDFAs()
	{
	    this.dfa14 = new DFA14(this);
	}

    static readonly short[] DFA14_eot = {
        -1, -1, -1, -1, -1, -1, -1, 27, -1, 27, -1, 32, 34, 36, -1, -1, 
        -1, -1, -1, -1, 27, -1, 38, -1, -1, 27, 27, -1, -1, 6, 27, -1, -1, 
        -1, -1, -1, -1, 27, -1, 27, 27, 8, 32, 27, 27, 47, 27, -1, 47
        };
    static readonly short[] DFA14_eof = {
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1
        };
    static readonly int[] DFA14_min = {
        9, 0, 0, 0, 0, 0, 0, 114, 0, 110, 0, 61, 61, 61, 0, 0, 0, 0, 0, 
        0, 111, 0, 46, 0, 0, 114, 97, 0, 0, 48, 100, 0, 0, 0, 0, 0, 0, 116, 
        0, 117, 108, 48, 48, 101, 115, 48, 101, 0, 48
        };
    static readonly int[] DFA14_max = {
        124, 0, 0, 0, 0, 0, 0, 114, 0, 110, 0, 61, 62, 61, 0, 0, 0, 0, 0, 
        0, 111, 0, 57, 0, 0, 114, 97, 0, 0, 122, 100, 0, 0, 0, 0, 0, 0, 
        116, 0, 117, 108, 122, 122, 101, 115, 122, 101, 0, 122
        };
    static readonly short[] DFA14_accept = {
        -1, 1, 2, 3, 4, 5, 6, -1, 7, -1, 8, -1, -1, -1, 14, 15, 16, 17, 
        18, 19, -1, 21, -1, 23, 24, -1, -1, 26, 27, -1, -1, 9, 20, 11, 10, 
        13, 12, -1, 22, -1, -1, -1, -1, -1, -1, -1, -1, 25, -1
        };
    static readonly short[] DFA14_special = {
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1
        };
    
    static readonly short[] dfa14_transition_null = null;

    static readonly short[] dfa14_transition0 = {
    	43
    	};
    static readonly short[] dfa14_transition1 = {
    	45
    	};
    static readonly short[] dfa14_transition2 = {
    	33, 31
    	};
    static readonly short[] dfa14_transition3 = {
    	27, 27, 27, 27, 27, 27, 27, 27, 27, 27, -1, -1, -1, -1, -1, -1, -1, 
    	    27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 
    	    27, 27, 27, 27, 27, 27, 27, 27, 27, 27, -1, -1, -1, -1, 27, -1, 
    	    27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 
    	    27, 27, 27, 27, 27, 27, 27, 27, 27, 27
    	};
    static readonly short[] dfa14_transition4 = {
    	46
    	};
    static readonly short[] dfa14_transition5 = {
    	44
    	};
    static readonly short[] dfa14_transition6 = {
    	48
    	};
    static readonly short[] dfa14_transition7 = {
    	28, 28, -1, 28, 28, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
    	    -1, -1, -1, -1, -1, -1, 28, 11, -1, 24, -1, 18, 8, 21, 1, 2, 16, 
    	    14, 5, 15, 23, 17, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, -1, 
    	    -1, 12, 10, 13, -1, -1, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 
    	    27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 
    	    3, -1, 4, 19, 27, -1, 9, 27, 27, 27, 27, 26, 27, 27, 27, 27, 27, 
    	    27, 27, 20, 7, 27, 27, 27, 27, 25, 27, 27, 27, 27, 27, 27, -1, 
    	    6
    	};
    static readonly short[] dfa14_transition8 = {
    	31
    	};
    static readonly short[] dfa14_transition9 = {
    	42
    	};
    static readonly short[] dfa14_transition10 = {
    	39
    	};
    static readonly short[] dfa14_transition11 = {
    	40
    	};
    static readonly short[] dfa14_transition12 = {
    	37
    	};
    static readonly short[] dfa14_transition13 = {
    	35
    	};
    static readonly short[] dfa14_transition14 = {
    	41
    	};
    static readonly short[] dfa14_transition15 = {
    	23, -1, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22
    	};
    static readonly short[] dfa14_transition16 = {
    	29
    	};
    static readonly short[] dfa14_transition17 = {
    	30
    	};
    
    static readonly short[][] DFA14_transition = {
    	dfa14_transition7,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition16,
    	dfa14_transition_null,
    	dfa14_transition17,
    	dfa14_transition_null,
    	dfa14_transition8,
    	dfa14_transition2,
    	dfa14_transition13,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition12,
    	dfa14_transition_null,
    	dfa14_transition15,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition10,
    	dfa14_transition11,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition3,
    	dfa14_transition14,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition_null,
    	dfa14_transition9,
    	dfa14_transition_null,
    	dfa14_transition0,
    	dfa14_transition5,
    	dfa14_transition3,
    	dfa14_transition3,
    	dfa14_transition1,
    	dfa14_transition4,
    	dfa14_transition3,
    	dfa14_transition6,
    	dfa14_transition_null,
    	dfa14_transition3
        };
    
    protected class DFA14 : DFA
    {
        public DFA14(BaseRecognizer recognizer) 
        {
            this.recognizer = recognizer;
            this.decisionNumber = 14;
            this.eot = DFA14_eot;
            this.eof = DFA14_eof;
            this.min = DFA14_min;
            this.max = DFA14_max;
            this.accept     = DFA14_accept;
            this.special    = DFA14_special;
            this.transition = DFA14_transition;
        }
    
        override public string Description
        {
            get { return "1:1: Tokens : ( T31 | T32 | T33 | T34 | T35 | OR | AND | EQUALS | NOTEQUALS | LT | LTEQ | GT | GTEQ | PLUS | MINUS | MULT | DIV | MOD | POW | NOT | STRING | INTEGER | FLOAT | DATETIME | BOOLEAN | IDENT | WS );"; }
        }
    
    }
    
 
    
}
