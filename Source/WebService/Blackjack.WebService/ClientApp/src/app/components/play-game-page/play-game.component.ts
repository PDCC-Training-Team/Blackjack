import { Component, OnInit } from "@angular/core";
import { UserApi } from "../../../shared/api/user.api";
import { UserDto } from "../../../shared/dtos/user.dto";
import { Dealer } from "../../types/dealer";
import { Deck } from "../../types/deck";
import { Player } from "../../types/player";

@Component({
  templateUrl: './play-game.component.html',
  styleUrls: ['./play-game.component.scss']
})
export class PlayGameComponent implements OnInit {

  /**
   * The UserDto object containing the logged in user's data
   */
  public user: UserDto;

  /**
   * Player object containing the player's information. 
   */
  public player: Player = new Player();

  /**
   * Dealer object containing the dealer's information.
   */
  public dealer: Dealer = new Dealer();

  /**
   * Deck object containing all possible cards that can still be drawn. 
   */
  public deck: Deck = new Deck();

  /**
   * The amount the player is betting on a single hand of blackjack
   */
  public wager: number = 0;

  /**
   * The number of decks the player is playing with.
   */
  public deckQty: number = 1;

  /**
   * Determines if the wager options div and play button will be hidden.
   */
  public isPregameHidden: boolean = false;

  /**
   * Determines if the entire Play Area div will be hidden.
   */
  public isPlayAreaHidden: boolean = true;

  /**
    * Determines if the Play Again button will be hidden.
    */
  public isPlayAgainHidden: boolean = true;

  /**
  * Determines if the Hit and Stay buttons will be hidden.
  */
  public isGameButtonHidden: boolean = true;
  /**
   * Explains to the player what play options are available as well
   * as the outcome of the game.
   */
  public nextMove: string;

  constructor(private _userApi: UserApi) {
  }

  /**
   * Removes predertimined amounts from the player's balance and
   * adds it to the upcoming hand's wager
   * @param chipValue
   */
  public _addToWager(chipValue: number): void {
    if (chipValue > this.user.balance) {
      alert('You have insufficient funds for this wager.');
      return;
    }
    this.wager += chipValue;
    this.user.balance -= chipValue;
  }

  /**
   * Returns funds to the player's balance and resets the wager to 0.
   */
  public _clearWager(): void {
    this.user.balance += this.wager;
    this.wager = 0;
  }

  /**
  * Begins the game by allowing players to enter their name and how many decks to play with.
  * Also determines if either the player, dealer, or both have black jack.
  */
  public async startGame(): Promise<void> {
    if (isNaN(this.deckQty) || this.deckQty < 1) {
      alert(`${this.deckQty} is not a valid amount of decks, please try again.`);
      return;
    }
    this.deck.shuffle(this.deckQty);
    this.player.updateHand(this.deck.deal(2));
    this.dealer.updateHand(this.deck.deal(2));
    this.isPregameHidden = true;
    this.isPlayAreaHidden = false;

    /*Checks for blackjacks. If anyone does, game automatically ends.*/
    if (this.player.hasBlackJack() && this.dealer.hasBlackJack()) {
      this.resolveBlackjack(BlackjackOutcomes.BothBlackjack, async () => this.pushOutcome());
      return;
    }
    else if (this.player.hasBlackJack() && !this.dealer.hasBlackJack()) {
      this.resolveBlackjack(BlackjackOutcomes.PlayerBlackjack, async () => this.winOutcome());
      return;
    }
    else if (!this.player.hasBlackJack() && this.dealer.hasBlackJack()) {
      this.resolveBlackjack(BlackjackOutcomes.DealerBlackjack, async () => this.loseOutcome());
      return;
    }
    this.nextMove = BlackjackOutcomes.PlayerChoice;
    this.isGameButtonHidden = false;
  }

  /**
   * Gives the player another card but automatically ends play if the player busts. 
   */
  public async hit(): Promise<void> {
    this.player.updateHand(this.deck.deal(1));
    if (this.player.hasBusted) {
      this.revealDealer();
      this.nextMove = BlackjackOutcomes.DealerWin;
      await this.loseOutcome();
      this.isPlayAgainHidden = false;
      this.isGameButtonHidden = true;
    }
  }

  // TODO: Use RxJS to perform interval work and move off setInterval
  /**
   * Ends the players turn, reveals dealer's cards, hides game buttons, and plays
   * the dealer's hand over two second intervals.
   */
  public async stay(): Promise<void> {
    this.revealDealer();
    const intervalID = setInterval(async () => {
      if (this.dealer.score > 17 || (this.dealer.score === 17 && !this.dealer.hasSoft17()) || this.dealer.hasBusted) {
        clearInterval(intervalID);
        if (this.dealer.score < this.player.score || this.dealer.hasBusted) {
          this.nextMove = BlackjackOutcomes.PlayerWin;
          await this.winOutcome();
        }
        else if (this.dealer.score > this.player.score) {
          this.nextMove = BlackjackOutcomes.DealerWin;
          await this.loseOutcome();
        }
        else {
          this.nextMove = BlackjackOutcomes.Push;
          await this.pushOutcome();
        }
        this.isPlayAgainHidden = false;
        return;
      }
      this.dealer.updateHand(this.deck.deal(1));
    }, 2000);
  }

  /**
   * Resets the player and dealer and starts a new game.
   */
  public playAgain(): void {
    this.isPregameHidden = false;
    this.isPlayAreaHidden = true;
    this.isPlayAgainHidden = true;
    this.isGameButtonHidden = true;
    this.player.reset();
    this.dealer.reset();
  }

  /**
   * Resolves any Blackjack situation by taking a Blackjack
   * outcome and an overall game outcome.
   * @param nextMove
   * @param outcomeFunc
   */
  public async resolveBlackjack(nextMove: BlackjackOutcomes, outcomeFunc: () => void): Promise<void> {
    this.revealDealer();
    this.nextMove = nextMove;
    await outcomeFunc();
    this.isPlayAgainHidden = false;
  }

  /**
   * Changes the dealer state to reveal his cards and prevent player from taking actions. 
   */
  public revealDealer(): void {
    this.dealer.isDealerTurn = true;
    this.dealer.calcStatus();
    this.isGameButtonHidden = true;
  }

  /**
   * Resolves any situation in which the player wins a hand of blackjack
   */
  public async winOutcome(): Promise<void> {
    this.user.balance += (this.wager * 2);
    this.wager = 0;
    await this._userApi.updateUser(this.user);
  }

  /**
   * Resolves any situation in which the player loses a hand of blackjack
   */
    public async loseOutcome(): Promise<void> {
    this.wager = 0;
    await this._userApi.updateUser(this.user);
  }

  /**
   * Resolves any situation in which the player ties in a hand of blackjack
   */
  public async pushOutcome(): Promise<void> {
    this.user.balance += this.wager;
    this.wager = 0;
    await this._userApi.updateUser(this.user);    
  }

  public async ngOnInit(): Promise<void> {
    this.user = new UserDto();
    this.user = await this._userApi.getUserByID(1);
    this.player.name = this.user.username;
  }
}

/**
 * Contains possible entries for the Next Move label.
 */
enum BlackjackOutcomes {
  BothBlackjack = 'You both have 21! It\'s a push! Play again?',
  PlayerBlackjack = 'You have 21! You win! Play again?',
  DealerBlackjack = 'Dealer has 21! You lose! Play again?',
  PlayerWin = 'You win! Play again?',
  DealerWin = 'Dealer wins! Play again?',
  PlayerChoice = 'What would you like to do?',
  Push = 'You tied. It\'s a push!'
}

