import { Component } from '@angular/core';
import { HeroSliderComponent } from "../../welcome-components/hero-slider-component/hero-slider-component";

@Component({
  selector: 'app-welcome',
  imports: [HeroSliderComponent],
  templateUrl: './welcome.html',
  styleUrl: './welcome.css',
})
export class Welcome {

}
