spaceX(0..xmax).
spaceY(0..ymax).
black(X,Y) :- hint(X,Y,Z).
white(X,Y) :- not black(X,Y), spaceX(X), spaceY(Y).
adjacent_h(X,Y,X,Y-1) :- Y > 0, spaceX(X), spaceY(Y).
adjacent_h(X,Y,X,Y+1) :- Y < ymax, spaceX(X), spaceY(Y).
adjacent_v(X,Y,X-1,Y) :- X > 0, spaceX(X), spaceY(Y).
adjacent_v(X,Y,X+1,Y) :- X < xmax, spaceX(X), spaceY(Y).
adjacent(X1,Y1,X2,Y2) :- adjacent_h(X1,Y1,X2,Y2).
adjacent(X1,Y1,X2,Y2) :- adjacent_v(X1,Y1,X2,Y2).
can_see_h(X,Y1,X,Y2) :- adjacent_h(X,Y1,X,Y2), not black(X,Y1; X,Y2), spaceX(X), spaceY(Y1; Y2), Y1!=Y2.
can_see_h(X,Y1,X,Y2) :- adjacent_h(X,Y1,X,Y3), can_see_h(X,Y3,X,Y2), not black(X,Y1; X,Y2; X,Y3), spaceX(X), spaceY(Y1; Y2; Y3), Y1!=Y2, Y2!=Y3.
can_see_v(X1,Y,X2,Y) :- adjacent_v(X1,Y,X2,Y), not black(X1,Y; X2,Y), spaceX(X1; X2), spaceY(Y), X1!=X2.
can_see_v(X1,Y,X2,Y) :- adjacent_v(X1,Y,X3,Y), can_see_v(X3,Y,X2,Y), not black(X1,Y; X2,Y; X3,Y), spaceX(X1; X2; X3), spaceY(Y), X1!=X2, X2!=X3.
can_see(X1,Y1,X2,Y2) :- can_see_v(X1,Y1,X2,Y2).
can_see(X1,Y1,X2,Y2) :- can_see_h(X1,Y1,X2,Y2).
:- lamp(X,Y), black(X,Y).
:- lamp(X1,Y1), lamp(X2,Y2), can_see(X1,Y1,X2,Y2).
%I{lamp(X1,Y1) : adjacent(X,Y,X1,Y1)}I :- hint(X,Y,I).
lit(X,Y) :- lamp(X1,Y1), can_see(X,Y,X1,Y1), not black(X,Y), spaceX(X;X1), spaceY(Y;Y1).
%lamp(X,Y) :- spaceX(X), spaceY(Y), not -lamp(X,Y).
%-lamp(X,Y) :- spaceX(X), spaceY(Y), not lamp(X,Y).
:- not lit(X,Y), not lamp(X,Y), not black(X,Y), spaceX(X), spaceY(Y).
#show lit/2.
