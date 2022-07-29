import { Gamer } from "./gamer";

export class Dealer extends Gamer {
  /**
   * Determines if the dealer is playing and whether his cards are revealed 
   */
  public isDealerTurn: boolean = false;

  public updateHand(cards: string[]): void {
    super.updateHand(cards);
    this.calcStatus();
  }

  /**
   * Updates the status to display the name, shown cards, and score of the player 
   */
  public calcStatus(): void {
    if (!this.isDealerTurn)
      this._status = `The dealer is showing ${this._hand[0]}`
    else
      super.calcStatus("The dealer");
  }

  public override reset(): void {
    this.isDealerTurn = false;
    super.reset();
  }

  /**
   * Determines if the dealer has a soft 17. 
   */
  public hasSoft17(): boolean {
    if (!this._hand.includes('A'))
      return false;
    let tempHand: string[] = this._hand.slice(0);
    const firstAceIndex: number = tempHand.indexOf('A');
    tempHand.splice(firstAceIndex, 1);
    let currentScore: number =
      tempHand.reduce((acc: number, curr: string) => {
        const cardValue: number = parseInt(curr, 10);
        return acc + (isNaN(cardValue)
          ? curr === 'A' ? 1 : 10
          : cardValue);
      }, 0);
    return currentScore + 11 === 17;
  }
}
