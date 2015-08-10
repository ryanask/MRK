/*************************************************************************
Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier

Contributors:
    * Sergey Bochkanov (ALGLIB project). Translation from C to
      pseudocode.

See subroutines comments for additional copyrights.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above copyright
  notice, this list of conditions and the following disclaimer listed
  in this license in the documentation and/or other materials
  provided with the distribution.

- Neither the name of the copyright holders nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*************************************************************************/

using System;

public class poissondistr
{
    /*************************************************************************
    Poisson distribution

    Returns the sum of the first k+1 terms of the Poisson
    distribution:

      k         j
      --   -m  m
      >   e    --
      --       j!
     j=0

    The terms are not summed directly; instead the incomplete
    gamma integral is employed, according to the relation

    y = pdtr( k, m ) = igamc( k+1, m ).

    The arguments must both be positive.
    ACCURACY:

    See incomplete gamma function

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double poissondistribution(int k,
        double m)
    {
        double result = 0;

        System.Diagnostics.Debug.Assert(k>=0 & m>0, "Domain error in PoissonDistribution");
        result = igammaf.incompletegammac(k+1, m);
        return result;
    }


    /*************************************************************************
    Complemented Poisson distribution

    Returns the sum of the terms k+1 to infinity of the Poisson
    distribution:

     inf.       j
      --   -m  m
      >   e    --
      --       j!
     j=k+1

    The terms are not summed directly; instead the incomplete
    gamma integral is employed, according to the formula

    y = pdtrc( k, m ) = igam( k+1, m ).

    The arguments must both be positive.

    ACCURACY:

    See incomplete gamma function

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double poissoncdistribution(int k,
        double m)
    {
        double result = 0;

        System.Diagnostics.Debug.Assert(k>=0 & m>0, "Domain error in PoissonDistributionC");
        result = igammaf.incompletegamma(k+1, m);
        return result;
    }


    /*************************************************************************
    Inverse Poisson distribution

    Finds the Poisson variable x such that the integral
    from 0 to x of the Poisson density is equal to the
    given probability y.

    This is accomplished using the inverse gamma integral
    function and the relation

       m = igami( k+1, y ).

    ACCURACY:

    See inverse incomplete gamma function

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double invpoissondistribution(int k,
        double y)
    {
        double result = 0;

        System.Diagnostics.Debug.Assert(k>=0 & y>=0 & y<1, "Domain error in InvPoissonDistribution");
        result = igammaf.invincompletegammac(k+1, y);
        return result;
    }
}
